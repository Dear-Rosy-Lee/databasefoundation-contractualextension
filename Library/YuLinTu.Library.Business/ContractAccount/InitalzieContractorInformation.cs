/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;


namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方信息处理
    /// </summary>
    partial class InitalizeLandSurveyInformation
    {
        #region Methods-Contractor

        /// <summary>
        /// 获取Excel表中信息
        /// </summary>
        /// <param name="allItem">所有项目</param>
        private void InitalizeFamilyInformation(LandFamily landFamily)
        {
            if (string.IsNullOrEmpty(landFamily.CurrentFamily.FamilyNumber))
            {
                landFamily.CurrentFamily.FamilyNumber = ToolString.ExceptSpaceString(GetString(allItem[currentIndex, 0]));
                if (string.IsNullOrEmpty(landFamily.CurrentFamily.FamilyNumber))
                {
                    string errorInformation = this.ExcelName + string.Format("表中第{0}行承包方编号未填写内容!", currentIndex + 1);
                    RecordErrorInformation(errorInformation);
                }
                else
                {
                    if (!ToolMath.MatchEntiretyNumber(landFamily.CurrentFamily.FamilyNumber))
                    {
                        string errorInformation = this.ExcelName + string.Format("表中第{0}行承包方编号{1}不符合数字类型要求!", currentIndex + 1, landFamily.CurrentFamily.FamilyNumber);
                        RecordErrorInformation(errorInformation);
                    }
                }
            }
            string familyName = (ContractLandImportSurveyDefine.NameIndex > 0 && ContractLandImportSurveyDefine.NameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NameIndex]) : "";
            string value = string.Empty;
            int count = -1;
            if (!string.IsNullOrEmpty(familyName))
            {
                landFamily.CurrentFamily.Name = familyName;
                InitializeInnerFamily(landFamily);
                value = (ContractLandImportSurveyDefine.NumberIndex > 0 && ContractLandImportSurveyDefine.NumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberIndex]) : "";
                if (!string.IsNullOrEmpty(value))
                {
                    int.TryParse(value, out count);
                }
                if (count == -1 && ContractLandImportSurveyDefine.NumberIndex >= 0)
                {
                    RecordErrorInformation(this.ExcelName + string.Format("表中{0}的家庭成员个数未填写内容!", familyName));
                }
                if (count == 0)
                {
                    RecordErrorInformation(this.ExcelName + string.Format("表中{0}的家庭成员个数{1}填写无效,不是有效数字!", familyName, value));
                }
                landFamily.PersonCount = count;
                landFamily.CurrentFamily.Telephone = (ContractLandImportSurveyDefine.TelephoneIndex > 0 && ContractLandImportSurveyDefine.TelephoneIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TelephoneIndex]) : "";
                if (!string.IsNullOrEmpty(landFamily.CurrentFamily.Telephone) && !ToolMath.MatchEntiretyNumber(landFamily.CurrentFamily.Telephone.Replace("-", "")))
                {
                    RecordErrorInformation(this.ExcelName + string.Format("表中{0}的联系号码{1}不满足数字要求!", familyName, landFamily.CurrentFamily.Telephone));
                }
                landFamily.CurrentFamily.TotalActualArea = (ContractLandImportSurveyDefine.TotalActualAreaIndex > 0 && ContractLandImportSurveyDefine.TotalActualAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TotalActualAreaIndex]) : "";
                landFamily.CurrentFamily.TotalAwareArea = (ContractLandImportSurveyDefine.TotalAwareAreaIndex > 0 && ContractLandImportSurveyDefine.TotalAwareAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TotalAwareAreaIndex]) : "";
                double tempTotalArea = 0.0;
                string temp = (ContractLandImportSurveyDefine.TotalTableAreaIndex > 0 && ContractLandImportSurveyDefine.TotalTableAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TotalTableAreaIndex]) : "";
                double.TryParse(temp, out tempTotalArea);
                landFamily.CurrentFamily.TotalTableArea = tempTotalArea;
                landFamily.CurrentFamily.TotalModoArea = (ContractLandImportSurveyDefine.TotalMotorizeAreaIndex > 0 && ContractLandImportSurveyDefine.TotalMotorizeAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TotalMotorizeAreaIndex]) : "";
                landFamily.CurrentFamily.Address = (ContractLandImportSurveyDefine.ContractorAddressIndex > 0 && ContractLandImportSurveyDefine.ContractorAddressIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ContractorAddressIndex]) : "";
                landFamily.CurrentFamily.PostalNumber = (ContractLandImportSurveyDefine.PostNumberIndex > 0 && ContractLandImportSurveyDefine.TelephoneIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.PostNumberIndex]) : "";
                if (!string.IsNullOrEmpty(landFamily.CurrentFamily.PostalNumber) && (!ToolMath.MatchEntiretyNumber(landFamily.CurrentFamily.PostalNumber) || landFamily.CurrentFamily.PostalNumber.Length != 6))
                {
                    RecordErrorInformation(this.ExcelName + string.Format("表中{0}的邮政编码{1}不满足6位数字要求!", familyName, landFamily.CurrentFamily.PostalNumber));
                }
                InitalizeFamilyExpandInformation(landFamily.CurrentFamily);//承包方扩展信息
                if (TableType == 10)
                {
                    landFamily.CurrentFamily.TotalModoArea = GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 10]);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(landFamily.CurrentFamily.Name))
                {
                    string errorInfo = this.ExcelName + string.Format("表序号为{0}的承包方姓名为空!", currentIndex + 1);
                    ReportErrorInfo(errorInfo);//记录错误信息
                }
            }
            AddPerson(landFamily);
            if (TableType == 10)
            {
                AddPersonExtendInformation(landFamily);
            }
        }

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        private LandFamily InitializeInnerFamily(LandFamily landFamily)
        {
            landFamily.CurrentFamily.ZoneCode = currentZone.FullCode;
            landFamily.CurrentFamily.ModifiedTime = DateTime.Now;
            landFamily.CurrentFamily.CreationTime = DateTime.Now;
            return landFamily;
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private bool AddPerson(LandFamily landFamily)
        {
            string value = string.Empty;
            Person person = new Person();
            person.FamilyID = landFamily.CurrentFamily.ID;
            person.CreateTime = DateTime.Now;
            person.LastModifyTime = DateTime.Now;
            person.Nation = eNation.UnKnown;
            person.ZoneCode = currentZone.FullCode;
            //性别
            value = (ContractLandImportSurveyDefine.NumberGenderIndex > 0 && ContractLandImportSurveyDefine.NumberGenderIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberGenderIndex]) : "";
            if (string.IsNullOrEmpty(value))
            {
                person.Gender = eGender.Unknow;
            }
            else
            {
                person.Gender = GetGender(value);
            }
            //年龄
            int age = 0;
            string strAge = (ContractLandImportSurveyDefine.NumberAgeIndex > 0 && ContractLandImportSurveyDefine.NumberAgeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberAgeIndex]) : "";
            Int32.TryParse(strAge, out age);
            //身份证号
            string icn = (ContractLandImportSurveyDefine.NumberIcnIndex > 0 && ContractLandImportSurveyDefine.NumberIcnIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberIcnIndex]) : "";
            //家庭关系
            person.Relationship = (ContractLandImportSurveyDefine.NumberRelatioinIndex > 0 && ContractLandImportSurveyDefine.NumberRelatioinIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberRelatioinIndex]) : "";
            //备注
            person.Comment = (ContractLandImportSurveyDefine.FamilyCommentIndex > 0 && ContractLandImportSurveyDefine.FamilyCommentIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyCommentIndex]) : "";
            person.Opinion = (ContractLandImportSurveyDefine.FamilyOpinionIndex > 0 && ContractLandImportSurveyDefine.FamilyOpinionIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyOpinionIndex]) : "";
            //名称
            string name = (ContractLandImportSurveyDefine.NumberNameIndex > 0 && ContractLandImportSurveyDefine.NumberNameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberNameIndex]) : "";
            if (string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(value) || age != 0 || !string.IsNullOrEmpty(icn)))
            {
                AddWarnMessage(this.ExcelName + string.Format("表序号为{0}的家庭成员姓名为空!", currentIndex + 1));
            }
            person.Name = name;
            InitalizeSharePersonCardType(person);
            if (string.IsNullOrEmpty(icn))
            {
                person.ICN = "";
                if (!string.IsNullOrEmpty(person.Name) && ContractLandImportSurveyDefine.NumberIcnIndex > 0)
                {
                    AddWarnMessage(this.ExcelName + "表中" + person.Name + "的身份证号码为空");
                }
            }
            else
            {
                person.ICN = icn;
                if (person.CardType == eCredentialsType.IdentifyCard)
                {
                    person = SetPersonValue(person);
                    if (!string.IsNullOrEmpty(icn) && icn.Length != 15 && icn.Length != 18)
                    {
                        ReportErrorInfo(this.ExcelName + string.Format("表中{0}的身份证号码{1}共{2}位,不满足身份证号码15位或18位数字要求!", person.Name, icn, icn.Length));
                    }
                    if (!string.IsNullOrEmpty(icn) && (icn.Length == 15 || icn.Length == 18) && !ToolMath.MatchEntiretyNumber(icn.Replace("x", "").Replace("X", "")))
                    {
                        ReportErrorInfo(this.ExcelName + string.Format("表中{0}的身份证号码{1}共{2}位,但不满足身份证号码数字要求!", person.Name, icn, icn.Length));
                    }
                }
            }
            if (person.Gender == eGender.Unknow && !string.IsNullOrEmpty(person.Name) && ContractLandImportSurveyDefine.NumberGenderIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + person.Name + "的性别填写不正确!");
            }
            //2011-2-18 weiwei  检查同户中存在同名成员的情况
            if (!string.IsNullOrEmpty(person.Name))
            {
                if (existPersons.ContainsKey(person.Name))
                {
                    AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下存在同名的成员：" + person.Name);
                }
                else
                {
                    existPersons.Add(person.Name, person.Name);
                }
            }
            if (!string.IsNullOrEmpty(person.Name))
            {
                SetPersonBirthDay(person, age);
                InitalizeSharePersonExpandInformation(person);
                landFamily.Persons.Add(person);
            }
            if (person.Name == landFamily.CurrentFamily.Name)
            {
                landFamily.CurrentFamily.SourceID = person.ID;
                landFamily.CurrentFamily.Number = person.ICN;
                landFamily.CurrentFamily.CardType = person.CardType;
                person.Relationship = "户主";//string.IsNullOrEmpty(person.Relationship) ? "户主" : person.Relationship;
                if (person.Name.Contains("集体"))
                {
                    person.Relationship = "本人";
                }
            }
            return true;
        }

        /// <summary>
        /// 设置共有人值
        /// </summary>
        private Person SetPersonValue(Person person)
        {
            if (string.IsNullOrEmpty(person.ICN))
            {
                return person;
            }
            if (ToolICN.Check(person.ICN))
            {
                switch (ToolICN.GetGender(person.ICN))
                {
                    case 1:
                        person.Gender = eGender.Male;
                        break;
                    case 0:
                        person.Gender = eGender.Female;
                        break;
                    default:
                        person.Gender = eGender.Unknow;
                        break;
                }
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            else
            {
                person.ICN = person.ICN.Trim();
                if (person.ICN.Length == 18 || person.ICN.Length == 15)
                {
                    if (person.Gender == eGender.Unknow)
                    {
                        switch (ToolICN.GetGenderInNotCheck(person.ICN))
                        {
                            case 1:
                                person.Gender = eGender.Male;
                                break;
                            case 0:
                                person.Gender = eGender.Female;
                                break;
                            default:
                                person.Gender = eGender.Unknow;
                                break;
                        }
                    }
                    person.Birthday = ToolICN.GetBirthdayInNotCheck(person.ICN);
                    AddWarnMessage(this.ExcelName + "表中" + person.Name + "的身份证号码：" + person.ICN + "不符合身份证验证规则!");
                }
            }
            return person;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="person"></param>
        private void SetPersonBirthDay(Person person, int age)
        {
            if (ContractLandImportSurveyDefine.NumberAgeIndex <= 0 && ContractLandImportSurveyDefine.AgeIndex <= 0)
            {
                return;
            }
            //计算年龄（虚岁）。会在之后从身份证号中更新年龄，此处计算的年龄仅当身份证号无效的情况下有效。
            if (age > 0 && (person.Birthday == null || !person.Birthday.HasValue))
            {
                person.Birthday = DateTime.Now.Date.AddYears(-age);
            }
            string birthDay = (ContractLandImportSurveyDefine.NumberAgeIndex > 0 && ContractLandImportSurveyDefine.NumberAgeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberAgeIndex]) : "";
            if (string.IsNullOrEmpty(birthDay))
            {
                return;
            }
            int year, month, day;
            year = month = day = 0;
            if (ContractLandImportSurveyDefine.NumberAgeIndex > 0)
            {
                Int32.TryParse(birthDay, out year);
                if (year == 0)
                {
                    return;
                }
                year = DateTime.Now.Year - year;
                month = day = 1;
            }
            if (ContractLandImportSurveyDefine.AgeIndex > 0)
            {
                string[] values = birthDay.Split(new char[] { '.' });
                if (values.Length != 3)
                {
                    return;
                }
                Int32.TryParse(values[0], out year);
                Int32.TryParse(values[1], out month);
                Int32.TryParse(values[2], out day);
            }
            try
            {
                person.Birthday = new DateTime(year, month, day);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        private eGender GetGender(string value)
        {
            if (value == "男")
                return eGender.Male;

            if (value == "女")
                return eGender.Female;
            return eGender.Unknow;
        }

        /// <summary>
        /// 添加承包方扩展信息
        /// </summary>
        private void AddPersonExtendInformation(LandFamily landFamily)
        {
            string value = string.Empty;
            Person person = new Person();
            person.FamilyID = landFamily.CurrentFamily.ID;
            person.CreateTime = DateTime.Now;
            person.LastModifyTime = DateTime.Now;
            person.Nation = eNation.UnKnown;
            person.ZoneCode = currentZone.FullCode;
            //性别
            value = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex + 3 < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 3]) : "";
            if (string.IsNullOrEmpty(value))
            {
                person.Gender = eGender.Unknow;
            }
            else
            {
                person.Gender = GetGender(value);
            }
            //是否共有人
            string icn = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex + 5 < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 5]) : "";
            //家庭关系
            person.Relationship = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex + 4 < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 4]) : "";
            //备注
            person.Comment = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex + 6 < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 6]) : "";
            //名称
            person.Name = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex + 2 < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 2]) : "";
            if (string.IsNullOrEmpty(icn))
            {
                person.ICN = "";
                if (!string.IsNullOrEmpty(person.Name))
                {
                    AddWarnMessage(this.ExcelName + "表中" + person.Name + "的身份证号码为空");
                }
            }
            else
            {
                person.ICN = icn;
                person = SetPersonValue(person);
                if (person.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(icn))
                {
                    if (icn.Length != 15 && icn.Length != 18)
                    {
                        ReportErrorInfo(this.ExcelName + string.Format("表中{0}的身份证号码{1}共{2}位,不满足身份证号码15位或18位数字要求!", person.Name, icn, icn.Length));
                    }
                    if ((icn.Length == 15 || icn.Length == 18) && !ToolMath.MatchEntiretyNumber(icn.Replace("x", "").Replace("X", "")))
                    {
                        ReportErrorInfo(this.ExcelName + string.Format("表中{0}的身份证号码{1}共{2}位,但不满足身份证号码数字要求!", person.Name, icn, icn.Length));
                    }
                }
            }
            if (person.Gender == eGender.Unknow && !string.IsNullOrEmpty(person.Name))
            {
                AddErrorMessage(this.ExcelName + "表中" + person.Name + "的性别" + value + "填写不正确!");
            }
            if (!string.IsNullOrEmpty(person.Name))
            {
                landFamily.TemporaryPersons.Add(person);
            }
        }

        /// <summary>
        /// 初始化承包方类型
        /// </summary>
        /// <param name="familyExpand"></param>
        private void InitalizeSharePersonCardType(Person person)
        {
            if (ContractLandImportSurveyDefine.NumberCartTypeIndex < 0)
            {
                person.CardType = eCredentialsType.IdentifyCard;
                return;
            }
            string objValue = (ContractLandImportSurveyDefine.NumberCartTypeIndex > 0 && ContractLandImportSurveyDefine.NumberCartTypeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NumberCartTypeIndex]) : "";
            string errorInformation = string.Empty;
            if (string.IsNullOrEmpty(person.Name))
            {
                return;
            }
            if (string.IsNullOrEmpty(objValue))
            {
                errorInformation = this.ExcelName + string.Format("表中{0}的证件类型未填写信息!", person.Name);
                RecordErrorInformation(errorInformation);
            }
            else
            {
                try
                {
                    person.CardType = (eCredentialsType)EnumNameAttribute.GetValue(typeof(eCredentialsType), objValue);
                }
                catch
                {
                    errorInformation = this.ExcelName + string.Format("表中{0}的证件类型{1}填写错误,不是{2}其中一种!", person.Name, objValue, InitalizeEnumDescription(typeof(eCredentialsType)));
                    RecordErrorInformation(errorInformation);
                }
            }
        }

        #endregion

        #region Methods-Extend

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeFamilyExpandInformation(VirtualPerson family)
        {
            VirtualPersonExpand familyExpand = new VirtualPersonExpand();
            familyExpand.ID = family.ID;
            familyExpand.Name = family.Name;
            familyExpand.HouseHolderName = family.Name;
            familyExpand.IsSourceContractor = (ContractLandImportSurveyDefine.IsSourceContractorIndex > 0 && ContractLandImportSurveyDefine.IsSourceContractorIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSourceContractorIndex]) : "";
            familyExpand.ContractorNumber = (ContractLandImportSurveyDefine.ContractorNumberIndex > 0 && ContractLandImportSurveyDefine.ContractorNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ContractorNumberIndex]) : "";
            familyExpand.LaborNumber = (ContractLandImportSurveyDefine.LaborNumberIndex > 0 && ContractLandImportSurveyDefine.LaborNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LaborNumberIndex]) : "";
            familyExpand.MoveFormerlyLandType = (ContractLandImportSurveyDefine.MoveFormerlyLandTypeIndex > 0 && ContractLandImportSurveyDefine.MoveFormerlyLandTypeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.MoveFormerlyLandTypeIndex]) : "";
            familyExpand.MoveFormerlyLandArea = (ContractLandImportSurveyDefine.MoveFormerlyLandAreaIndex > 0 && ContractLandImportSurveyDefine.MoveFormerlyLandAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.MoveFormerlyLandAreaIndex]) : "";
            familyExpand.AllocationPerson = (ContractLandImportSurveyDefine.AllocationPersonIndex > 0 && ContractLandImportSurveyDefine.AllocationPersonIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.AllocationPersonIndex]) : "";
            familyExpand.FarmerNature = (ContractLandImportSurveyDefine.FarmerNatureIndex > 0 && ContractLandImportSurveyDefine.FarmerNatureIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FarmerNatureIndex]) : "";
            familyExpand.SurveyPerson = (ContractLandImportSurveyDefine.FamilySurveyPersonIndex > 0 && ContractLandImportSurveyDefine.FamilySurveyPersonIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilySurveyPersonIndex]) : "";
            string cellValue = (ContractLandImportSurveyDefine.FamilySurveyDateIndex > 0 && ContractLandImportSurveyDefine.FamilySurveyDateIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilySurveyDateIndex]) : "";
            familyExpand.SurveyDate = (ContractLandImportSurveyDefine.FamilySurveyDateIndex > 0 && ContractLandImportSurveyDefine.FamilySurveyDateIndex < columnCount) ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.FamilySurveyDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (familyExpand.SurveyDate == null || !familyExpand.SurveyDate.HasValue))
            {
                string information = this.ExcelName + string.Format("表中{0}的承包方调查日期{1}不符合日期填写要求!", familyExpand.HouseHolderName, cellValue);
                AddErrorMessage(information);
            }
            familyExpand.SurveyChronicle = (ContractLandImportSurveyDefine.FamilySurveyChronicleIndex > 0 && ContractLandImportSurveyDefine.FamilySurveyChronicleIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilySurveyChronicleIndex]) : "";
            familyExpand.CheckPerson = (ContractLandImportSurveyDefine.FamilyCheckPersonIndex > 0 && ContractLandImportSurveyDefine.FamilyCheckPersonIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyCheckPersonIndex]) : "";
            cellValue = (ContractLandImportSurveyDefine.FamilyCheckDateIndex > 0 && ContractLandImportSurveyDefine.FamilyCheckDateIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyCheckDateIndex]) : "";
            familyExpand.CheckDate = (ContractLandImportSurveyDefine.FamilyCheckDateIndex > 0 && ContractLandImportSurveyDefine.FamilyCheckDateIndex < columnCount) ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyCheckDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (familyExpand.CheckDate == null || !familyExpand.CheckDate.HasValue))
            {
                string information = this.ExcelName + string.Format("表中{0}的承包方审核日期{1}不符合日期填写要求!", familyExpand.HouseHolderName, cellValue);
                AddErrorMessage(information);
            }
            familyExpand.CheckOpinion = (ContractLandImportSurveyDefine.FamilyCheckOpinionIndex > 0 && ContractLandImportSurveyDefine.FamilyCheckOpinionIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FamilyCheckOpinionIndex]) : "";
            familyExpand.ConcordNumber = (ContractLandImportSurveyDefine.SecondConcordNumberIndex > 0 && ContractLandImportSurveyDefine.SecondConcordNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondConcordNumberIndex]) : "";
            familyExpand.WarrantNumber = (ContractLandImportSurveyDefine.SecondWarrantNumberIndex > 0 && ContractLandImportSurveyDefine.SecondWarrantNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondWarrantNumberIndex]) : "";
            cellValue = (ContractLandImportSurveyDefine.StartTimeIndex > 0 && ContractLandImportSurveyDefine.StartTimeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.StartTimeIndex]) : "";
            familyExpand.ConcordStartTime = (ContractLandImportSurveyDefine.StartTimeIndex > 0 && ContractLandImportSurveyDefine.StartTimeIndex < columnCount) ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.StartTimeIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (familyExpand.ConcordStartTime == null || !familyExpand.ConcordStartTime.HasValue))
            {
                string information = this.ExcelName + string.Format("表中{0}的承包方合同开始日期{1}不符合日期填写要求!", familyExpand.HouseHolderName, cellValue);
                AddErrorMessage(information);
            }
            cellValue = (ContractLandImportSurveyDefine.EndTimeIndex > 0 && ContractLandImportSurveyDefine.EndTimeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.EndTimeIndex]) : "";
            familyExpand.ConcordEndTime = (ContractLandImportSurveyDefine.EndTimeIndex > 0 && ContractLandImportSurveyDefine.EndTimeIndex < columnCount) ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.EndTimeIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (familyExpand.ConcordEndTime == null || !familyExpand.ConcordEndTime.HasValue))
            {
                string information = this.ExcelName + string.Format("表中{0}的承包方合同结束日期{1}不符合日期填写要求!", familyExpand.HouseHolderName, cellValue);
                AddErrorMessage(information);
            }

            string constructType = (ContractLandImportSurveyDefine.VPConstructModeIndex > 0 && ContractLandImportSurveyDefine.VPConstructModeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.VPConstructModeIndex]) : "";
            if (constructType != "")
            {
                eConstructMode constructMode = (eConstructMode)EnumNameAttribute.GetValue(typeof(eConstructMode), constructType);
                familyExpand.ConstructMode = constructMode;
            }
            InitalizeContractorType(familyExpand);
            family.FamilyExpand = familyExpand;  //扩展信息
            ReportSurveyDateInformation(familyExpand);
            ReportSurveyConcordDateInformation(familyExpand);
            familyExpand = null;
        }

        /// <summary>
        /// 报告调查日期信息
        /// </summary>
        private void ReportSurveyDateInformation(VirtualPersonExpand expand)
        {
            if (ContractLandImportSurveyDefine.FamilySurveyDateIndex < 0 || ContractLandImportSurveyDefine.FamilyCheckDateIndex < 0)
            {
                return;
            }
            if (expand.SurveyDate == null || !expand.SurveyDate.HasValue || expand.CheckDate == null || !expand.CheckDate.HasValue)
            {
                return;
            }
            if (expand.SurveyDate.Value > expand.CheckDate.Value)
            {
                string errorInformation = this.ExcelName + string.Format("表中{0}的调查日期{1}大于审核日期{2}!", expand.HouseHolderName, ToolDateTime.GetLongDateString(expand.SurveyDate.Value), ToolDateTime.GetLongDateString(expand.CheckDate.Value));
                RecordErrorInformation(errorInformation);
            }
        }

        /// <summary>
        /// 报告合同日期信息
        /// </summary>
        private void ReportSurveyConcordDateInformation(VirtualPersonExpand expand)
        {
            if (ContractLandImportSurveyDefine.StartTimeIndex < 0 || ContractLandImportSurveyDefine.EndTimeIndex < 0)
            {
                return;
            }
            if (expand.ConcordStartTime == null || !expand.ConcordStartTime.HasValue || expand.ConcordEndTime == null || !expand.ConcordEndTime.HasValue)
            {
                return;
            }
            if (expand.ConcordStartTime.Value > expand.ConcordEndTime.Value)
            {
                string errorInformation = this.ExcelName + string.Format("表中{0}的合同起始日期{1}大于合同结束日期{2}!", expand.HouseHolderName, ToolDateTime.GetLongDateString(expand.ConcordStartTime.Value), ToolDateTime.GetLongDateString(expand.ConcordEndTime.Value));
                RecordErrorInformation(errorInformation);
            }
        }

        /// <summary>
        /// 初始化承包方类型
        /// </summary>
        /// <param name="familyExpand"></param>
        private void InitalizeContractorType(VirtualPersonExpand familyExpand)
        {
            if (ContractLandImportSurveyDefine.ContractorTypeIndex < 0)
            {
                familyExpand.ContractorType = familyExpand.Name.Contains("集体") ? eContractorType.Unit : eContractorType.Farmer;
                return;
            }
            string value = (ContractLandImportSurveyDefine.ContractorTypeIndex > 0 && ContractLandImportSurveyDefine.ContractorTypeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ContractorTypeIndex]) : "";
            string errorInformation = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                errorInformation = this.ExcelName + string.Format("表中{0}的承包方类型未填写信息!", familyExpand.HouseHolderName);
                RecordErrorInformation(errorInformation);
            }
            else
            {
                try
                {
                    familyExpand.ContractorType = (eContractorType)EnumNameAttribute.GetValue(typeof(eContractorType), value);
                }
                catch
                {
                    errorInformation = this.ExcelName + string.Format("表中{0}的承包方类型{1}填写错误,不是{2}其中一种!", familyExpand.HouseHolderName, value, InitalizeEnumDescription(typeof(eContractorType)));
                    RecordErrorInformation(errorInformation);
                }
            }
            if (familyExpand.Name.Contains("集体"))
            {
                familyExpand.ContractorType = eContractorType.Unit;
            }
        }

        /// <summary>
        /// 初始化共有人扩展信息
        /// </summary>
        private void InitalizeSharePersonExpandInformation(Person person)
        {
            string value = (ContractLandImportSurveyDefine.NationIndex > 0 && ContractLandImportSurveyDefine.NationIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NationIndex]) : "";
            if (!string.IsNullOrEmpty(value) && value.Length == 1)
            {
                value += "族";
            }
            object obj = EnumNameAttribute.GetValue(typeof(eNation), value);
            if (obj != null)
            {
                person.Nation = (eNation)EnumNameAttribute.GetValue(typeof(eNation), value);
            }
            value = null;
            obj = null;
            person.AccountNature = (ContractLandImportSurveyDefine.AccountNatureIndex > 0 && ContractLandImportSurveyDefine.AccountNatureIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.AccountNatureIndex]) : "";
            if (ContractLandImportSurveyDefine.IsSharedLandIndex > 0)
            {
                person.IsSharedLand = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex]) : "";
                string record = "";
                if (string.IsNullOrEmpty(person.IsSharedLand))
                {
                    record = this.ExcelName + string.Format("表中{0}的是否共有人列未填写数据!", person.Name);
                }
                else
                {
                    if (person.IsSharedLand != "是" && person.IsSharedLand != "否")
                    {
                        record = this.ExcelName + string.Format("表中{0}的是否共有人列数据{1}填写错误,内容不是是或否!", person.Name, person.IsSharedLand);
                    }
                }
                if (!string.IsNullOrEmpty(record))
                {
                    RecordErrorInformation(record);
                }
            }
            else
            {
                person.IsSharedLand = "是";
            }
            person.CencueComment = (ContractLandImportSurveyDefine.CencueCommentIndex > 0 && ContractLandImportSurveyDefine.CencueCommentIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CencueCommentIndex]) : "";
            person.SourceMove = (ContractLandImportSurveyDefine.SourceMoveIndex > 0 && ContractLandImportSurveyDefine.SourceMoveIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SourceMoveIndex]) : "";
            person.MoveTime = (ContractLandImportSurveyDefine.MoveTimeIndex > 0 && ContractLandImportSurveyDefine.MoveTimeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.MoveTimeIndex]) : "";
            person.IsNinetyNineSharePerson = (ContractLandImportSurveyDefine.IsNinetyNineSharePersonIndex > 0 && ContractLandImportSurveyDefine.IsNinetyNineSharePersonIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsNinetyNineSharePersonIndex]) : "";
        }

        #endregion
    }
}
