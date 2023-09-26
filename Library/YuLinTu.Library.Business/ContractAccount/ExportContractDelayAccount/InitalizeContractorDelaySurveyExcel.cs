using System;
using System.Collections.Generic;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化导出数据
    /// </summary>
    public partial class ExportContractorDelaySurveyExcel
    {
        /// <summary>
        /// 开始书写数据
        /// </summary>
        private void BeginWrite()
        {
            try
            {
                WriteTitle();//写标题信息
                WriteContent();//开始写内容
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 书写承包方信息
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="ts"></param>
        private int WritePerson(List<Person> ps, List<Person> ts, VirtualPerson vp, VirtualPerson sp, int height)
        {
            int pindex = index;
            int age = 0;//家庭成员年龄
            int curIndex = columnIndex;
            int familyIndex = columnIndex + 1;
            int secondIndex = columnIndex + 1;
            VirtualPersonExpand familyExpand = vp.FamilyExpand;
            if (ts == null)
                ts = new List<Person>();

            foreach (Person person in ps)
            {
                person.Name = InitalizeFamilyName(person.Name, SystemSet.KeepRepeatFlag);
                columnIndex = curIndex;
                if (contractLandOutputSurveyDefine.NumberNameValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, (person.Name == vp.Name && TableType == 5) ? VirtualPersonOperator.InitalizeFamilyName(person.Name, SystemSet.KeepRepeatFlag) : person.Name);//成员姓名
                }
                string value = EnumNameAttribute.GetDescription(person.Gender);
                if (contractLandOutputSurveyDefine.NumberGenderValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, value == "未知" ? "" : value);//成员性别
                }
                age = person.GetAge();
                if (contractLandOutputSurveyDefine.NumberAgeValue)
                {
                    columnIndex++;
                    if (age > 0 && age < 120)
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, age.ToString());//成员年龄
                    else
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, "");//成员年龄
                }
                if (contractLandOutputSurveyDefine.NumberCartTypeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, EnumNameAttribute.GetDescription(person.CardType));//证件类型
                }
                if (contractLandOutputSurveyDefine.NumberIcnValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.ICN);//成员身份证号
                }
                if (contractLandOutputSurveyDefine.NumberRelatioinValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Relationship);//家庭成员关系
                }
                if (contractLandOutputSurveyDefine.AgeValue)
                {
                    columnIndex++;
                    string birthDay = (person.Birthday != null && person.Birthday.HasValue) ? ToolDateTime.GetShortDateString(person.Birthday.Value) : "";
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, birthDay);//初始日期
                }
                if (contractLandOutputSurveyDefine.NationValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, EnumNameAttribute.GetDescription(person.Nation));//民族
                }
                if (contractLandOutputSurveyDefine.AccountNatureValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.AccountNature);//户口性质
                }
                if (contractLandOutputSurveyDefine.IsSharedLandValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.IsSharedLand == "否" ? "否" : "是");//是否享有承包地
                }
                if (contractLandOutputSurveyDefine.FamilyCommentValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Comment);//备注
                }
                if (contractLandOutputSurveyDefine.FamilyOpinionValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Opinion);//备注
                }
                if (contractLandOutputSurveyDefine.IsSourceContractorValue)
                {
                    columnIndex++;
                    if (pindex - index == ps.Count - 1)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + height - 1), familyExpand != null ? familyExpand.IsSourceContractor : "");//是否是原承包户
                    }
                }
                if (contractLandOutputSurveyDefine.ContractorNumberValue)
                {
                    columnIndex++;
                    if (pindex - index == ps.Count - 1)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + height - 1), familyExpand != null ? familyExpand.ContractorNumber : "");//现承包人数
                    }
                }
                if (contractLandOutputSurveyDefine.LaborNumberValue)
                {
                    columnIndex++;
                    if (pindex - index == ps.Count - 1)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + height - 1), familyExpand != null ? familyExpand.LaborNumber : "");//总劳力数
                    }
                }
                if (contractLandOutputSurveyDefine.CencueCommentValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.CencueComment);//备注
                }
                if (contractLandOutputSurveyDefine.FarmerNatureValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, familyExpand != null ? familyExpand.FarmerNature : "");//农户性质
                }
                if (contractLandOutputSurveyDefine.SourceMoveValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.SourceMove);//从何处迁入
                }
                if (contractLandOutputSurveyDefine.MoveTimeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.MoveTime);//迁入时间
                }
                if (contractLandOutputSurveyDefine.MoveFormerlyLandTypeValue)
                {
                    columnIndex++;
                    if (pindex - index == ps.Count - 1)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + height - 1), familyExpand != null ? familyExpand.MoveFormerlyLandType : "");//迁入前土地类型
                    }
                }
                if (contractLandOutputSurveyDefine.MoveFormerlyLandAreaValue)
                {
                    columnIndex++;
                    if (pindex - index == ps.Count - 1)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + height - 1), familyExpand != null ? familyExpand.MoveFormerlyLandArea : "");//迁入前土地面积
                    }
                }
                if (contractLandOutputSurveyDefine.IsNinetyNineSharePersonValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.IsNinetyNineSharePerson);//是否为99年共有人
                }
                if (contractLandOutputSurveyDefine.SecondNameValue)
                {
                    familyIndex = columnIndex;
                    columnIndex++;
                }
                if (contractLandOutputSurveyDefine.SecondNumberValue)
                {
                    if (!contractLandOutputSurveyDefine.SecondNameValue)
                    {
                        familyIndex = columnIndex;
                    }
                    columnIndex++;
                }
                secondIndex = columnIndex;
                Person tablePerson = ts.Find(per => per.Name == person.Name);
                if (tablePerson != null)
                {
                    WriteTablePerson(tablePerson, pindex);
                    ts.Remove(tablePerson);
                }
                else
                {
                    WriteTableInformation();
                }
                pindex++;
            }
            foreach (Person pson in ts)
            {
                columnIndex = curIndex;
                WriteContractorInformation();
                WriteTablePerson(pson, pindex);
                pindex++;
            }
            columnIndex = secondIndex;
            WriteTablePersonInformation(index, sp, height - 1);
            return familyIndex;
        }

        /// <summary>
        /// 书写二轮延包人口信息
        /// </summary>
        /// <param name="person"></param>
        /// <param name="pindex"></param>
        private void WriteTablePerson(Person person, int pindex)
        {
            if (contractLandOutputSurveyDefine.SecondNumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Name);//成员姓名
            }
            string value = EnumNameAttribute.GetDescription(person.Gender);
            if (contractLandOutputSurveyDefine.SecondNumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, value == "未知" ? "" : value);//成员性别
            }
            int age = person.GetAge();
            if (contractLandOutputSurveyDefine.SecondNumberAgeValue)
            {
                columnIndex++;
                if (age > 0 && age < 120)
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, age.ToString());//成员年龄
                else
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, "");//成员年龄
            }
            if (contractLandOutputSurveyDefine.SecondNumberIcnValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.ICN);//成员身份证号
            }
            if (contractLandOutputSurveyDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Relationship);//家庭成员关系
            }
            if (contractLandOutputSurveyDefine.SecondNationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, EnumNameAttribute.GetDescription(person.Nation));//民族
            }
            if (contractLandOutputSurveyDefine.SecondAgeValue)
            {
                columnIndex++;
                string birthday = person.Birthday != null && person.Birthday.HasValue ? ToolDateTime.GetShortDateString(person.Birthday.Value) : "";
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, birthday);//出生日期
            }
            if (contractLandOutputSurveyDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FirstContractAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FoodCropAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Name);//成员姓名
            }

            if (contractLandOutputSurveyDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.IsDeaded);//成员已死亡
            }
            if (contractLandOutputSurveyDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.LocalMarriedRetreatLand);//出嫁后未退承包地人员
            }
            if (contractLandOutputSurveyDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.PeasantsRetreatLand);//农转非后未退承包地人员
            }
            if (contractLandOutputSurveyDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.ForeignMarriedRetreatLand);//婚进但在婚出地未退承包地人员
            }
            if (contractLandOutputSurveyDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, person.Comment);//备注
            }
        }

        /// <summary>
        /// 书写二轮延包信息
        /// </summary>
        private void WriteTableInformation()
        {
            if (contractLandOutputSurveyDefine.SecondNumberNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberGenderValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberAgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberIcnValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNationValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondAgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FirstContractAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FoodCropAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ExPackageNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ExPackageNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.IsDeadedValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 书写二轮延包人口信息
        /// </summary>
        /// <param name="person"></param>
        /// <param name="pindex"></param>
        private void WriteTablePersonInformation(int pindex, VirtualPerson family, int rowCount)
        {
            VirtualPersonExpand familyExpand = null;
            if (family != null)
            {
                familyExpand = family.FamilyExpand;
            }
            if (contractLandOutputSurveyDefine.SecondNumberNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberGenderValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberAgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberIcnValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNationValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondAgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (pindex + rowCount), familyExpand != null ? familyExpand.FirstContractorPersonNumber : null);//一轮承包人数
            }
            if (contractLandOutputSurveyDefine.FirstContractAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (pindex + rowCount), familyExpand != null ? familyExpand.FirstContractArea : null);//一轮承包面积
            }
            if (contractLandOutputSurveyDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (pindex + rowCount), familyExpand != null ? familyExpand.SecondContractorPersonNumber : null);//二轮承包人数
            }
            if (contractLandOutputSurveyDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (pindex + rowCount), familyExpand != null ? familyExpand.SecondExtensionPackArea : null);//二轮延包面积
            }
            if (contractLandOutputSurveyDefine.FoodCropAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + pindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (pindex + rowCount), familyExpand != null ? familyExpand.FoodCropArea : null);//粮食种植面积
            }
            if (contractLandOutputSurveyDefine.ExPackageNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ExPackageNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.IsDeadedValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 书写承包方信息
        /// </summary>
        private void WriteContractorInformation()
        {
            if (contractLandOutputSurveyDefine.NumberNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NumberGenderValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NumberAgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NumberCartTypeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NumberIcnValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NumberRelatioinValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.AgeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.NationValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.AccountNatureValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.IsSharedLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FamilyCommentValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FamilyOpinionValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.AccountNatureValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.IsSourceContractorValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.ContractorNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.LaborNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.CencueCommentValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.FarmerNatureValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SourceMoveValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.MoveTimeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandTypeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.IsNinetyNineSharePersonValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondNumberValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 书写承包地块信息
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="hight"></param>
        /// <param name="telephone"></param>
        private int WriteContractLand(List<ContractLand> cs, int hight, int cindexx, string telephone, List<ContractConcord> concords)
        {
            if (cs == null || cs.Count < 0)
            {
                return -1;
            }
            int cindex = cindexx;
            int telephoneIndex = cindex;
            string plan = string.Empty;
            string ment = string.Empty;
            int tempIndex = cindex;//3.25使用 最后需要再次打印
            double tempActualAreaAllCount = 0.0;//3.25使用 最后需要再次打印
            double tempAwareAreaCount = 0.0;//3.25使用 最后需要再次打印
            double tempMotorizeLandAreaCount = 0.0;//3.25使用 最后需要再次打印
            double tempTotalTableAreaCount = 0.0;//3.25使用 最后需要再次打印
            double tempContractDelayCount = 0.0;
            int curIndex = columnIndex;

            foreach (ContractLand land in cs)
            {
                columnIndex = curIndex;
                //3.25需求
                if (concord.ID == Guid.Empty)
                {
                    ActualAreaAllCount += land.ActualArea;
                    AwareAreaCount += land.AwareArea;
                    MotorizeLandAreaCount += land.MotorizeLandArea == null ? land.ActualArea - land.AwareArea : land.MotorizeLandArea.Value;
                    TotalTableAreaCount += land.TableArea == null ? 0.0 : land.TableArea.Value;
                }
                //单户的各种总面积
                tempContractDelayCount += land.ContractDelayArea;
                tempActualAreaAllCount += land.ActualArea;
                tempAwareAreaCount += land.AwareArea;
                //tempMotorizeLandAreaCount += land.MotorizeLandArea == null ? land.ActualArea - land.AwareArea : land.MotorizeLandArea.Value;
                tempMotorizeLandAreaCount += land.MotorizeLandArea == null ? 0 : land.MotorizeLandArea.Value;
                tempTotalTableAreaCount += land.TableArea == null ? 0.0 : land.TableArea.Value;
                if (contractLandOutputSurveyDefine.LandNameValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.Name);//地块名称
                }
                if (contractLandOutputSurveyDefine.CadastralNumberValue)
                {
                    columnIndex++;
                    //string landNumber = land.LandNumber;
                    //if (landNumber.Length > SystemSet.LandNumericFormatValueSet)
                    //{
                    //    landNumber = landNumber.Substring(SystemSet.LandNumericFormatValueSet);
                    //}
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandNumber);//地块编码
                }
                if (contractLandOutputSurveyDefine.ImageNumberValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.ImageNumber);//图幅编号
                }
                if (contractLandOutputSurveyDefine.TableAreaValue)
                {
                    columnIndex++;
                    if (land.TableArea != null && land.TableArea.Value > 0.0)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2));//二轮台账面积
                        onlyTotalTableAreaCount += land.TableArea.Value;
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, SystemSet.InitalizeAreaString());//二轮台账面积
                    }
                }

                if (contractLandOutputSurveyDefine.TotalTableAreaValue)
                {
                    columnIndex++;
                    if (concord.ID != Guid.Empty && concord.IsValid)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), concord.TotalTableArea == null ? SystemSet.InitalizeAreaString() :
                            concord.TotalTableArea.Value > 0.0 ? ToolMath.SetNumbericFormat(concord.TotalTableArea.Value.ToString(), 2) : SystemSet.InitalizeAreaString());//台账总面积
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), tempTotalTableAreaCount > 0.0 ? ToolMath.SetNumbericFormat(tempTotalTableAreaCount.ToString(), 2) : SystemSet.InitalizeAreaString());//二轮台账总面积
                    }
                }
                if (contractLandOutputSurveyDefine.PlotNumberValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.PlotNumber);//畦数
                }

                if (concords == null || concords.Count == 0)
                {
                    if (contractLandOutputSurveyDefine.ConcordValue)
                    {
                        columnIndex++;
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex - 1) + index, PublicityConfirmDefine.GetColumnValue(columnIndex - 1) + (index + high - 1), "");//合同编号
                    }
                }
                if (concords != null && concords.Count == 1)
                {
                    if (contractLandOutputSurveyDefine.ConcordValue)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), concords[0].ConcordNumber);//合同编号
                    }
                    if (contractLandOutputSurveyDefine.RegeditBookValue)
                    {
                        columnIndex++;
                        if (BookColletion != null)
                        {
                            Library.Entity.ContractRegeditBook book = BookColletion.Find(t => t.ID == concords[0].ID);
                            SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), book == null ? "" : book.RegeditNumber);//证书编号
                        }
                    }
                }

                if (contractLandOutputSurveyDefine.ExPackageNumberValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), hight.ToString());
                }
                if (contractLandOutputSurveyDefine.AwareAreaValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.AwareArea > 0.0 ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : SystemSet.InitalizeAreaString());//确权面积
                }
                onlyAwareAreaCount += land.AwareArea;
                if (contractLandOutputSurveyDefine.TotalAwareAreaValue)
                {
                    columnIndex++;
                    if (concord.ID != Guid.Empty && concord.IsValid)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), concord.CountAwareArea > 0.0 ? ToolMath.SetNumbericFormat(concord.CountAwareArea.ToString(), 2) : SystemSet.InitalizeAreaString());//确权总面积
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), tempAwareAreaCount > 0.0 ? ToolMath.SetNumbericFormat(tempAwareAreaCount.ToString(), 2) : SystemSet.InitalizeAreaString());//确权总面积2011-7-22更改为合同总确权面积
                    }
                }
                if (contractLandOutputSurveyDefine.ActualAreaValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.ActualArea > 0.0 ? ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2) : SystemSet.InitalizeAreaString());//实测面积
                }
                ActualAreaCount += land.ActualArea;

                //3.25新需求  对于导图之后 没有合同 没有机动地的情况需要手动导出总面积及算出机动地面积
                if (contractLandOutputSurveyDefine.TotalActualAreaValue)
                {
                    columnIndex++;
                    if (concord.ID != Guid.Empty && concord.IsValid)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), concord.CountActualArea > 0.0 ? ToolMath.SetNumbericFormat(concord.CountActualArea.ToString(), 2) : SystemSet.InitalizeAreaString());//实测总面积
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), tempActualAreaAllCount > 0.0 ? ToolMath.SetNumbericFormat(tempActualAreaAllCount.ToString(), 2) : SystemSet.InitalizeAreaString());//实测总面积
                    }
                }
                if (contractLandOutputSurveyDefine.ContractDelayAreaValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.ContractDelayArea > 0.0 ? ToolMath.SetNumbericFormat(land.ContractDelayArea.ToString(), 2) : SystemSet.InitalizeAreaString());
                }
                ContractDelayCount += land.ContractDelayArea;
                if (contractLandOutputSurveyDefine.TotalContractDelayAreaValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), tempContractDelayCount > 0.0 ? ToolMath.SetNumbericFormat(tempContractDelayCount.ToString(), 2) : SystemSet.InitalizeAreaString());//延包总面积
                }

                if (contractLandOutputSurveyDefine.MotorizeAreaValue)
                {
                    columnIndex++;
                    if (land.MotorizeLandArea == null || land.MotorizeLandArea.Value <= 0.0)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, SystemSet.InitalizeAreaString());//机动地面积
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, ToolMath.SetNumbericFormat(land.MotorizeLandArea.Value.ToString(), 2));//机动地面积
                        onlyMotorizeLandAreaCount += land.MotorizeLandArea.Value;
                    }
                }
                if (contractLandOutputSurveyDefine.TotalMotorizeAreaValue)
                {
                    columnIndex++;
                    //if (concord.ID != Guid.Empty && concord.IsValid)
                    //{
                    //    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1),   concord.CountMotorizeLandArea > 0.0 ? ToolMath.SetNumbericFormat(concord.CountMotorizeLandArea.ToString(), 2) : AgricultureSetting.InitalizeAreaString());//机动地总面积
                    //}
                    //else
                    //{
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + hight - 1), tempMotorizeLandAreaCount > 0.0 ? ToolMath.SetNumbericFormat(tempMotorizeLandAreaCount.ToString(), 2) : SystemSet.InitalizeAreaString());//机动地总面积
                    //}
                }
                //四至
                string[] nei = new string[] { land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth };

                if (contractLandOutputSurveyDefine.LandNeighborValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, nei.Length >= 1 ? nei[0] : "");//东
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, nei.Length >= 2 ? nei[1] : "");//南
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, nei.Length >= 3 ? nei[2] : "");//西
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, nei.Length >= 4 ? nei[3] : "");//北
                }
                if (contractLandOutputSurveyDefine.LandPurposeValue)
                {
                    columnIndex++;
                    string purpose = string.Empty;
                    if (!string.IsNullOrEmpty(land.Purpose) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.Purpose && t.GroupCode == DictionaryTypeInfo.TDYT);
                        if (dic != null)
                        {
                            purpose = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, purpose);//土地用途
                }
                if (contractLandOutputSurveyDefine.LandLevelValue)
                {
                    columnIndex++;
                    string landLevel = string.Empty;
                    if (!string.IsNullOrEmpty(land.LandLevel) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.LandLevel && t.GroupCode == DictionaryTypeInfo.DLDJ);
                        if (dic != null)
                        {
                            if (dic.Code == ((int)eLandLevel.UnKnow).ToString())
                                landLevel = "";
                            else
                                landLevel = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landLevel);//等级
                }
                if (contractLandOutputSurveyDefine.LandTypeValue)
                {
                    columnIndex++;
                    if (string.IsNullOrEmpty(land.LandName) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.LandCode && t.GroupCode == DictionaryTypeInfo.TDLYLX);
                        if (dic != null)
                        {
                            plan = dic.Name;
                        }
                    }
                    else
                    {
                        plan = land.LandName;
                    }
                    if (!string.IsNullOrEmpty(plan) && plan == "未知")
                        plan = "";
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, plan);//地类
                }
                if (contractLandOutputSurveyDefine.IsFarmerLandValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, (land.IsFarmerLand != null && land.IsFarmerLand.HasValue) ? (land.IsFarmerLand.Value ? "是" : "否") : "");//是否基本农田
                }
                if (contractLandOutputSurveyDefine.ReferPersonValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.ReferPerson);//指界人
                }
                if (contractLandOutputSurveyDefine.ArableTypeValue)
                {
                    columnIndex++;
                    string landCategory = string.Empty;
                    if (!string.IsNullOrEmpty(land.LandCategory) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.LandCategory && t.GroupCode == DictionaryTypeInfo.DKLB);
                        if (dic != null)
                        {
                            landCategory = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landCategory);//地块类别
                }
                if (contractLandOutputSurveyDefine.ConstructModeValue)
                {
                    columnIndex++;
                    string constructMode = string.Empty;
                    if (!string.IsNullOrEmpty(land.ConstructMode) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.ConstructMode && t.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                        if (dic != null)
                        {
                            constructMode = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, constructMode);//承包方式
                }

                if (contractLandOutputSurveyDefine.PlatTypeValue)
                {
                    columnIndex++;
                    string platType = string.Empty;
                    if (!string.IsNullOrEmpty(land.PlatType) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.PlatType && t.GroupCode == DictionaryTypeInfo.ZZLX);
                        if (dic != null)
                        {
                            platType = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, platType);//种植类型platType == "其它" ? "" :
                }
                if (contractLandOutputSurveyDefine.ManagementTypeValue)
                {
                    columnIndex++;
                    string managementType = string.Empty;
                    if (!string.IsNullOrEmpty(land.ManagementType) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.ManagementType && t.GroupCode == DictionaryTypeInfo.JYFS);
                        if (dic != null)
                        {
                            managementType = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, managementType == "未知" ? "" : managementType); //经营方式
                }
                if (contractLandOutputSurveyDefine.LandPlantValue)
                {
                    columnIndex++;
                    string plantType = string.Empty;
                    if (!string.IsNullOrEmpty(land.ManagementType) && DictionList != null)
                    {
                        Dictionary dic = DictionList.Find(t => !string.IsNullOrEmpty(t.Code) && !string.IsNullOrEmpty(t.GroupCode) &&
                        t.Code == land.PlantType && t.GroupCode == DictionaryTypeInfo.GBZL);
                        if (dic != null)
                        {
                            plantType = dic.Name;
                        }
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, plantType == "未知" ? "" : plantType);//耕保类型
                }
                if (contractLandOutputSurveyDefine.SourceNameValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.FormerPerson);//原耕种人
                }
                if (contractLandOutputSurveyDefine.LandLocationValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.ExtendA);//宗地座落方位描述
                }
                cindex++;
            }
            return telephoneIndex;
        }

        /// <summary>
        /// 书写地块扩展信息
        /// </summary>
        private void WriteLandExpandInformation(List<ContractLand> cs, int hight)
        {
            if (cs == null || cs.Count < 1)
            {
                return;
            }
            int cindex = index;
            int curIndex = columnIndex;
            foreach (ContractLand land in cs)
            {
                columnIndex = curIndex;
                if (contractLandOutputSurveyDefine.CommentValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.Comment);//备注
                }
                if (contractLandOutputSurveyDefine.OpinionValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.Opinion);
                }
                if (contractLandOutputSurveyDefine.IsTransterValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.IsTransfer ? "是" : "否");//是否流转
                }
                if (contractLandOutputSurveyDefine.TransterModeValue)
                {
                    columnIndex++;
                    Dictionary dic = null;
                    if (DictionList != null)
                    {
                        dic = DictionList.Find(t => t.GroupCode == DictionaryTypeInfo.LZLX && t.Code == land.TransferType);
                    }
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, dic == null ? "" : dic.Name);//流转方式
                }
                if (contractLandOutputSurveyDefine.TransterTermValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.TransferTime);//流转期限
                }
                if (contractLandOutputSurveyDefine.TransterAreaValue)
                {
                    columnIndex++;
                    string landArea = (!string.IsNullOrEmpty(land.ExtendA) && !string.IsNullOrEmpty(land.ExtendC)) ? "" : (land.PertainToArea > 0 ? land.PertainToArea.ToString() : "");
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landArea);//流转面积
                }
                if (contractLandOutputSurveyDefine.UseSituationValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand != null ? land.LandExpand.UseSituation : "");//利用情况
                }
                if (contractLandOutputSurveyDefine.YieldValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand != null ? land.LandExpand.Yield : "");//产量情况
                }
                if (contractLandOutputSurveyDefine.OutputValueValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand != null ? land.LandExpand.OutputValue : "");//产值情况
                }
                if (contractLandOutputSurveyDefine.IncomeSituationValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand != null ? land.LandExpand.IncomeSituation : "");//收益情况
                }
                cindex++;
            }
        }

        /// <summary>
        /// 地块调查信息
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="hight"></param>
        private void WriteLandSurveyInformation(List<ContractLand> cs, int hight)
        {
            if (cs == null || cs.Count < 1)
            {
                return;
            }
            int cindex = index;
            int curIndex = columnIndex;
            foreach (ContractLand land in cs)
            {
                columnIndex = curIndex;
                if (contractLandOutputSurveyDefine.LandSurveyPersonValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.SurveyPerson);
                }
                if (contractLandOutputSurveyDefine.LandSurveyDateValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, (land.LandExpand.SurveyDate != null && land.LandExpand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.SurveyDate.Value) : "");
                }
                if (contractLandOutputSurveyDefine.LandSurveyChronicleValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.SurveyChronicle);
                }
                if (contractLandOutputSurveyDefine.LandCheckPersonValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.CheckPerson);
                }
                if (contractLandOutputSurveyDefine.LandCheckDateValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, (land.LandExpand != null && land.LandExpand.CheckDate != null && land.LandExpand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.CheckDate.Value) : "");
                }
                if (contractLandOutputSurveyDefine.LandCheckOpinionValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandExpand.CheckOpinion);
                }
                cindex++;
            }
        }

        /// <summary>
        /// 书写承包地块信息
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="hight"></param>
        /// <param name="telephone"></param>
        private void WriteSecondTableLand(List<SecondTableLand> cs, int hight, string allArea)
        {
            if (cs == null || cs.Count < 1)
            {
                InitalizeSecondLandInformation();
                return;
            }
            int cindex = index;
            int tempIndex = cindex;//3.25使用 最后需要再次打印
            int curIndex = columnIndex;
            foreach (var land in cs)
            {
                AgricultureLandExpand landExpand = land.LandExpand;
                columnIndex = curIndex;
                //单户的各种总面积
                secondTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                if (contractLandOutputSurveyDefine.SecondLandNumberValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landExpand != null ? landExpand.SecondLandNumber : "");//地块编码
                }
                if (contractLandOutputSurveyDefine.SecondLandNameValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.Name);//地块名称
                }
                if (contractLandOutputSurveyDefine.SecondLandTypeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.LandName);//地类
                }
                if (contractLandOutputSurveyDefine.SecondArableTypeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landExpand != null ? landExpand.SecondLandType : "");//土地类型
                }
                if (contractLandOutputSurveyDefine.SecondLandLevelValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landExpand != null ? landExpand.SecondLandLevel : "");//土地等级
                }
                if (contractLandOutputSurveyDefine.SecondTableAreaValue)
                {
                    columnIndex++;
                    if (land.TableArea != null && land.TableArea.Value > 0.0)
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2));//二轮台账面积
                    }
                    else
                    {
                        SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, SystemSet.InitalizeAreaString());//二轮台账面积
                    }
                }
                if (contractLandOutputSurveyDefine.SecondTotalTableAreaValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + tempIndex, PublicityConfirmDefine.GetColumnValue(columnIndex) + (tempIndex + high - 1), !string.IsNullOrEmpty(allArea) ? ToolMath.SetNumbericFormat(allArea, 2) : "");//台账总面积
                }
                //四至
                if (contractLandOutputSurveyDefine.SecondLandNeighborValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.NeighborEast);//东
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.NeighborSouth);//南
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.NeighborWest);//西
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.NeighborNorth);//北
                }
                if (contractLandOutputSurveyDefine.SecondIsFarmerLandValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landExpand != null ? landExpand.SecondIsFarmerLand : "");//是否基本农田
                }
                if (contractLandOutputSurveyDefine.SecondLandPurposeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, landExpand != null ? landExpand.SecondLandPurpose : "");//土地用途
                }
                if (contractLandOutputSurveyDefine.SecondCommentValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, PublicityConfirmDefine.GetColumnValue(columnIndex) + cindex, land.Comment);//备注
                }
                cindex++;
            }
        }

        /// <summary>
        /// 初始化二轮地块信息
        /// </summary>
        private void InitalizeSecondLandInformation()
        {
            if (contractLandOutputSurveyDefine.SecondLandNumberValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondLandNameValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondLandTypeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondArableTypeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondLandLevelValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondTableAreaValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondTotalTableAreaValue)
            {
                columnIndex++;
            }
            //四至
            if (contractLandOutputSurveyDefine.SecondLandNeighborValue)
            {
                columnIndex++;
                columnIndex++;
                columnIndex++;
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondIsFarmerLandValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondLandPurposeValue)
            {
                columnIndex++;
            }
            if (contractLandOutputSurveyDefine.SecondCommentValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 书写序号
        /// </summary>
        /// <param name="high"></param>
        /// <param name="number"></param>
        /// <param name="HouseholderName"></param>
        /// <param name="Count"></param>
        private void InitalizeContractorInformation(int high, string number, string HouseholderName, string Count, VirtualPerson item)
        {
            try
            {
                //SetRange("A" + index, "A" + (index + high - 1), number);
                SetRange("A" + index, "A" + (index + high - 1), number);//编号
                SetRange("B" + index, "B" + (index + high - 1), TableType == 5 ? VirtualPersonOperator.InitalizeFamilyName(HouseholderName, SystemSet.KeepRepeatFlag) : HouseholderName);//户主
                if (contractLandOutputSurveyDefine.ContractorTypeValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), (item.FamilyExpand != null) ? EnumNameAttribute.GetDescription(item.FamilyExpand.ContractorType) : "");//承包方类型
                }
                if (contractLandOutputSurveyDefine.NumberValue)
                {
                    columnIndex++;
                    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), !string.IsNullOrEmpty(item.PersonCount) ? Count : "0");//家庭成员个数
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 书写其他信息
        /// </summary>
        private void WriteFamilyExpandInformation(int high, VirtualPerson family, VirtualPersonExpand expand)
        {
            if (expand == null)
            {
                return;
            }
            if (contractLandOutputSurveyDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), family.FamilyExpand.AllocationPerson);
            }
            if (contractLandOutputSurveyDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), family.Address);
            }
            if (contractLandOutputSurveyDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), family.PostalNumber);
            }
            if (contractLandOutputSurveyDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), family.Telephone);
            }
            if (contractLandOutputSurveyDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), expand != null ? expand.ConcordNumber : "");
            }
            if (contractLandOutputSurveyDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), expand != null ? expand.WarrantNumber : "");
            }
            if (contractLandOutputSurveyDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), (expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue) ? ToolDateTime.GetLongDateString(expand.ConcordStartTime.Value) : "");
            }
            if (contractLandOutputSurveyDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), (expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue) ? ToolDateTime.GetLongDateString(expand.ConcordEndTime.Value) : "");
            }
            if (contractLandOutputSurveyDefine.ConstructTypeValue)
            {
                columnIndex++;
                string constructType = ((int)expand.ConstructMode).ToString();
                Dictionary dic = null;
                if (DictionList != null)
                    dic = DictionList.Find(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS && t.Code == constructType);
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), dic != null ? dic.Name : "");
            }
            if (contractLandOutputSurveyDefine.FamilySurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), family.FamilyExpand.SurveyPerson);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), (expand.SurveyDate != null && expand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(expand.SurveyDate.Value) : "");
            }
            if (contractLandOutputSurveyDefine.FamilySurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), expand.SurveyChronicle);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), expand.CheckPerson);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), (expand.CheckDate != null && expand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(expand.CheckDate.Value) : "");
            }
            if (contractLandOutputSurveyDefine.FamilyCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + (index + high - 1), expand.CheckOpinion);
            }
        }

        /// <summary>
        /// 书写序号
        /// </summary>
        /// <param name="high"></param>
        /// <param name="number"></param>
        /// <param name="HouseholderName"></param>
        /// <param name="Count"></param>
        private void WriteTableNumber(int high, string number, string HouseholderName, string count, int familyIndex)
        {
            if (contractLandOutputSurveyDefine.SecondNameValue)
            {
                familyIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(familyIndex) + index, PublicityConfirmDefine.GetColumnValue(familyIndex) + (index + high - 1), HouseholderName);//户主姓名
            }

            if (contractLandOutputSurveyDefine.SecondNumberValue)
            {
                familyIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(familyIndex) + index, PublicityConfirmDefine.GetColumnValue(familyIndex) + (index + high - 1), count);//家庭成员个数
            }
        }

        /// <summary>
        /// 书写承包地信息
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="high"></param>
        private void WriteContract(List<ContractLand> cs, int high, List<ContractConcord> concords)
        {
        }

        /// <summary>
        /// 对共有人进行排序-更改过
        /// </summary>
        /// <param name="personCollection"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            if (personCollection == null || personCollection.Count == 0)
            {
                return new List<Person>();
            }
            List<Person> sharePersonCollection = new List<Person>();
            foreach (var person in personCollection)
            {
                if (person.Name == houseName)
                {
                    sharePersonCollection.Add(person);
                    break;
                }
            }
            foreach (var person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        /// <summary>
        /// 比较现实与二轮延包家庭成员数
        /// </summary>
        /// <param name="localPerson"></param>
        /// <param name="tablePerson"></param>
        /// <returns></returns>
        private int ComparePersonValue(List<Person> localPerson, List<Person> tablePerson)
        {
            foreach (Person person in localPerson)
            {
                Person per = tablePerson.Find(pr => pr.Name == person.Name);
                if (per != null)
                {
                    tablePerson.Remove(per);
                }
            }
            return localPerson.Count + tablePerson.Count;
        }

        /// <summary>
        /// 书写结束信息
        /// </summary>
        private void WriteCount()
        {
            columnIndex = 2;
            SetRange("A" + index, "A" + index, "合计");//合计
            if (contractLandOutputSurveyDefine.ContractorTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PersonCount.ToString(), true);//PersonCount
            }
            if (contractLandOutputSurveyDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberCartTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.AgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.NationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.AccountNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsSharedLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilyOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsSourceContractorValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ContractorNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LaborNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.CencueCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FarmerNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);//农户性质
            }
            if (contractLandOutputSurveyDefine.SourceMoveValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.MoveTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsNinetyNineSharePersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberIcnValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondNationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FirstContractAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FoodCropAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ExPackageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, packageCount > 0 ? packageCount.ToString() : "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.CadastralNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, landCount > 0 ? landCount.ToString() : "", true);
            }
            if (contractLandOutputSurveyDefine.ImageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.TableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, onlyTotalTableAreaCount > 0 ? onlyTotalTableAreaCount.AreaFormat() : "\\", true);//ToolMath.SetNumbericFormat(onlyTotalTableAreaCount.ToString(), 2)
            }
            if (contractLandOutputSurveyDefine.TotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, TotalTableAreaCount > 0 ? TotalTableAreaCount.AreaFormat() : "", true);
            }
            if (contractLandOutputSurveyDefine.ActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, ActualAreaCount > 0 ? ActualAreaCount.AreaFormat() : "\\", true);//ToolMath.SetNumbericFormat(ActualAreaCount.ToString(), 2)
            }

            if (contractLandOutputSurveyDefine.TotalActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, ActualAreaAllCount > 0 ? ActualAreaAllCount.AreaFormat() : "", true);
            }
            if (contractLandOutputSurveyDefine.ContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, ContractDelayCount > 0 ? ContractDelayCount.AreaFormat() : "\\", true);//ToolMath.SetNumbericFormat(ActualAreaCount.ToString(), 2)
            }

            if (contractLandOutputSurveyDefine.TotalContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, TotalContractDelayCount > 0 ? TotalContractDelayCount.AreaFormat() : "", true);
            }
            if (contractLandOutputSurveyDefine.AwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, onlyAwareAreaCount > 0 ? ToolMath.SetNumbericFormat(onlyAwareAreaCount.ToString(), 2) : "", true);
            }
            if (contractLandOutputSurveyDefine.TotalAwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, AwareAreaCount > 0 ? ToolMath.SetNumbericFormat(AwareAreaCount.ToString(), 2) : "", true);
            }
            if (contractLandOutputSurveyDefine.MotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, onlyMotorizeLandAreaCount > 0 ? ToolMath.SetNumbericFormat(onlyMotorizeLandAreaCount.ToString(), 2) : "", true);
            }
            if (contractLandOutputSurveyDefine.TotalMotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, MotorizeLandAreaCount > 0 ? ToolMath.SetNumbericFormat(MotorizeLandAreaCount.ToString(), 2) : "", true);
            }

            if (contractLandOutputSurveyDefine.LandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ReferPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }

            if (contractLandOutputSurveyDefine.ConstructModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.PlotNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.PlatTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ManagementTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandPlantValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SourceNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandLocationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.ConcordValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.RegeditBookValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.IsTransterValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.TransterModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.TransterTermValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.CommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.OpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, secondTableArea > 0 ? ToolMath.SetNumbericFormat(secondTableArea.ToString(), 2) : "", true);
            }
            if (contractLandOutputSurveyDefine.SecondTotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, secondTotalTableArea > 0 ? ToolMath.SetNumbericFormat(secondTotalTableArea.ToString(), 2) : "", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondIsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.SecondCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandSurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandSurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandSurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (TableType == 3 || TableType == 5)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + index, PublicityConfirmDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
        }
    }
}