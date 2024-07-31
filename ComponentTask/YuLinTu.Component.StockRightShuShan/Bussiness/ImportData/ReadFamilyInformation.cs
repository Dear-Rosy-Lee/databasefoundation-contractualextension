/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

#define PERSON_NAME_EMPTY    //共有人姓名为空时不执行导入

using System;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Component.StockRightShuShan.Model;

namespace YuLinTu.Component.StockRightShuShan.Bussiness
{
    /// <summary>
    /// 读取数据
    /// </summary>
    public partial class ReadExcelBase : ExcelBase
    {
        public string errorInformation = string.Empty;
        private List<string> _contractorNumList = new List<string>();//承包方编号表

        #region 读取承包方数据

        /// <summary>
        /// 设置家庭信息
        /// </summary>
        /// <param name="family">户对象</param>
        /// <param name="familyName">户主名</param>
        /// <param name="allItem">所有值</param>
        public void GetContractorInfo(VirtualPerson curFamily, ExcelReadEntity readEntity)
        {
            VirtualPersonExpand expand = new VirtualPersonExpand();
            var familyName = readEntity.ContractorName;
            var currentIndex = readEntity.StartRow;
            var contractorNumber = GetString(_allItem[currentIndex, ColumnDefine.CONTRACTOR_NUMBER]);//字段：承包方编号
            curFamily.FamilyNumber = contractorNumber;
            curFamily.Name = familyName;//字段：承包方名称
            curFamily.ZoneCode = _currentZone.FullCode;
            curFamily.PersonCount = readEntity.SharePersonCount.ToString();
            curFamily.Address = GetString(_allItem[currentIndex, ColumnDefine.ADDRESS]);//字段：承包方地址
            curFamily.PostalNumber = GetString(_allItem[currentIndex, ColumnDefine.POSTAL_CODE]);//字段：邮政编码
            if (!string.IsNullOrEmpty(curFamily.PostalNumber) && (!ToolMath.MatchEntiretyNumber(curFamily.PostalNumber) || curFamily.PostalNumber.Length != 6))
            {
                RecordErrorInformation(string.Format("表中{0}的邮政编码{1}不满足6位数字要求!", familyName, curFamily.PostalNumber));
            }
            curFamily.Telephone = GetString(_allItem[currentIndex, ColumnDefine.TEL]);//字段：联系电话
            if (!string.IsNullOrEmpty(curFamily.Telephone) && !ToolMath.MatchEntiretyNumber(curFamily.Telephone.Replace("-", "")))
            {
                RecordErrorInformation(string.Format("表中{0}的联系电话{1}不满足数字要求!", familyName, curFamily.PostalNumber));
            }

            expand.HouseHolderName = familyName;
            expand.SurveyPerson = GetString(_allItem[currentIndex, ColumnDefine.SURVEY_PERSON]);//字段：调查员
            string cellValue = GetString(_allItem[currentIndex, 17]);
            expand.SurveyDate = GetDateTime(_allItem[currentIndex, ColumnDefine.SURVEY_DATE]);//字段：调查日期
            if (!string.IsNullOrEmpty(cellValue) && (expand.SurveyDate == null || !expand.SurveyDate.HasValue))
            {
                string information = string.Format("表中{0}的承包方调查日期{1}不符合日期填写要求!", expand.HouseHolderName, cellValue);
                RecordErrorInformation(information);
            }
            var remark = GetString(_allItem[currentIndex, ColumnDefine.SURVEY_REMARK]);//字段：调查记事

            expand.SurveyChronicle = remark;

            expand.CheckPerson = GetString(_allItem[currentIndex, ColumnDefine.CHECK_PERSON]);//字段：审核人
            expand.ConcordNumber = GetString(_allItem[currentIndex, ColumnDefine.CONCORD_NUMBER]);//字段：承包合同编号
            expand.WarrantNumber = GetString(_allItem[currentIndex, ColumnDefine.WARRANT_NUMBER]);// 字段：经营权证编号
            expand.ExtendName = GetString(_allItem[currentIndex, ColumnDefine.LAND_XISHU]);//字段：系数   确股信息系数暂时存放在承包方的扩展ExtendName里面

            InitalizeFamilyType(curFamily, readEntity);
            curFamily.IsStockFarmer = true;
            curFamily.FamilyExpand = expand;
            ReportSurveyDateInformation(curFamily.FamilyExpand);
            ReportSurveyConcordDateInformation(curFamily.FamilyExpand);
        }

        /// <summary>
        /// 获取承包方式
        /// </summary>
        private eConstructMode GetConstructMode(string modeName)
        {
            eConstructMode mode = eConstructMode.Family;
            modeName = modeName.Trim();
            object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), modeName);
            if (obj != null)
                mode = (eConstructMode)obj;
            return mode;
        }

        /// <summary>
        /// 报告调查日期信息
        /// </summary>
        private void ReportSurveyDateInformation(VirtualPersonExpand expand)
        {
            if (expand.SurveyDate == null || !expand.SurveyDate.HasValue || expand.CheckDate == null || !expand.CheckDate.HasValue)
            {
                return;
            }
            if (expand.SurveyDate.Value > expand.CheckDate.Value)
            {
                string errorInformation = string.Format("表中{0}的调查日期{1}大于审核日期{2}!", expand.HouseHolderName, ToolDateTime.GetLongDateString(expand.SurveyDate.Value), ToolDateTime.GetLongDateString(expand.CheckDate.Value));
                RecordErrorInformation(errorInformation);
            }
        }

        /// <summary>
        /// 报告合同日期信息
        /// </summary>
        private void ReportSurveyConcordDateInformation(VirtualPersonExpand expand)
        {
            if (expand.ConcordStartTime == null || !expand.ConcordStartTime.HasValue || expand.ConcordEndTime == null || !expand.ConcordEndTime.HasValue)
            {
                return;
            }
            if (expand.ConcordStartTime.Value > expand.ConcordEndTime.Value)
            {
                string errorInformation = string.Format("表中{0}的合同起始日期{1}大于合同结束日期{2}!", expand.HouseHolderName, ToolDateTime.GetLongDateString(expand.ConcordStartTime.Value), ToolDateTime.GetLongDateString(expand.ConcordEndTime.Value));
                RecordErrorInformation(errorInformation);
            }
        }

        /// <summary>
        /// 初始化承包方类型
        /// </summary>
        private void InitalizeFamilyType(VirtualPerson contractor, ExcelReadEntity readEntity)
        {
            var expand = contractor.FamilyExpand;
            string value = GetString(_allItem[readEntity.StartRow, 2]);//字段：承包方类型
            string errorInformation = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                errorInformation = string.Format("表中{0}的承包方类型未填写信息!", contractor.Name);
                RecordErrorInformation(errorInformation);
            }
            else
            {
                try
                {
                    expand.ContractorType = (eContractorType)EnumNameAttribute.GetValue(typeof(eContractorType), value);
                }
                catch
                {
                    errorInformation = string.Format("表中{0}的承包方类型{1}填写错误,不是{2}其中一种!", contractor.Name, value, InitalizeEnumDescription(typeof(eContractorType)));
                    RecordErrorInformation(errorInformation);
                }
            }
        }

        /// <summary>
        /// 初始化枚举类型字符串
        /// </summary>
        /// <returns></returns>
        private string InitalizeEnumDescription(Type enumType)
        {
            EnumNameAttribute[] values = EnumNameAttribute.GetAttributes(enumType);
            string description = "";
            for (int i = 0; i < values.Length; i++)
            {
                description += values[i].Description;
                description += "、";
            }
            values = null;
            return description.Substring(0, description.Length - 1);
        }

        /// <summary>
        /// 设置家庭信息
        /// </summary>
        /// <param name="family">户对象</param>
        /// <param name="familyName">户主名</param>
        /// <param name="allItem">所有值</param>
        private void GetSharePersonList(ConvertEntity convertyEntity, ExcelReadEntity readEntity)
        {
            List<Person> personList = new List<Person>();
            int currentIndex = readEntity.StartRow;
            for (int i = 0; i < readEntity.SharePersonCount; i++, currentIndex++)
            {
                Person person = new Person();
                var personName = GetString(_allItem[currentIndex, ColumnDefine.PERSON_NAME]);//字段：共有人姓名
#if PERSON_NAME_EMPTY
                if (string.IsNullOrWhiteSpace(personName))//共有人姓名为空时不执行导入
                {
                    continue;
                }
#endif

                person.Name = personName;//人名
                string identifyNumber = GetString(_allItem[currentIndex, ColumnDefine.PERSON_ID_NUMBER]);
                string objValue = GetString(_allItem[currentIndex, ColumnDefine.PERSON_ID_TYPE]);
                string errorInformation = string.Empty;
                if (string.IsNullOrEmpty(objValue))
                {
                    errorInformation = string.Format("表中{0}的证件类型未填写信息!", personName);
                    RecordErrorInformation(errorInformation);
                }
                else
                {
                    try
                    {
                        person.CardType = (eCredentialsType)EnumNameAttribute.GetValue(typeof(eCredentialsType), objValue);//字段：证件类型
                    }
                    catch
                    {
                        errorInformation = string.Format("表中{0}的证件类型{1}填写错误,不是{2}其中一种!", personName, objValue, InitalizeEnumDescription(typeof(eCredentialsType)));
                        RecordErrorInformation(errorInformation);
                    }
                }
                person.ICN = SetPersonIdNumber(personName, identifyNumber, person.CardType);//字段：身份证号码
                person.Gender = eGender.Unknow;
                if (ToolICN.Check(person.ICN) && person.CardType == eCredentialsType.IdentifyCard)
                {
                    person.Gender = ToolICN.GetGender(person.ICN) == 1 ? eGender.Male : eGender.Female;
                    person.Birthday = ToolICN.GetBirthday(person.ICN);
                }
                else
                {
                    person.Gender = eGender.Unknow;
                    string ageValue = GetString(_allItem[currentIndex, ColumnDefine.PERSON_GENDER]);
                    int age = 0;
                    int.TryParse(ageValue, out age);
                    if (age > 0)
                    {
                        person.Birthday = new DateTime(DateTime.Now.Year - age, 1, 1);
                    }
                    else
                    {
                        person.Birthday = null;
                    }
                }
                person.Age = GetAge(person).ToString();
                person.Nation = eNation.Han;
                if (person.Gender == eGender.Unknow)
                {
                    string value = GetString(_allItem[currentIndex, ColumnDefine.PERSON_GENDER]);
                    if (value == "男")
                    {
                        person.Gender = eGender.Male;//字段：性别
                    }
                    else if (value == "女")
                    {
                        person.Gender = eGender.Female;
                    }
                }
                person.FamilyID = convertyEntity.Contractor.ID;
                person.Comment = GetString(_allItem[currentIndex, ColumnDefine.PERSON_COMMENT]);//字段：备注

                var relationship = GetString(_allItem[currentIndex, ColumnDefine.PERSON_RELATION]);//字段：家庭关系
                person.Relationship = relationship;
                if (!string.IsNullOrEmpty(relationship) && relationship.Trim().Equals("户主"))//如果是户主则设置相关成本方信息
                {
                    convertyEntity.Contractor.Number = person.ICN;
                    convertyEntity.Contractor.CardType = person.CardType;
                    convertyEntity.Contractor.Comment = person.Comment;
                }

                person.IsSharedLand = GetString(_allItem[currentIndex, ColumnDefine.PERSON_ISSHARE]);//字段：是否共有人
                string record = "";
                if (string.IsNullOrEmpty(person.IsSharedLand))
                {
                    record = string.Format("表中{0}的是否共有人列未填写数据!", person.Name);
                }
                else if (person.IsSharedLand != "是" && person.IsSharedLand != "否")
                {
                    record = string.Format("表中{0}的是否共有人列数据{1}填写填写错误,内容不是是或否!", person.Name, person.IsSharedLand);
                }

                if (!string.IsNullOrEmpty(record))
                {
                    RecordErrorInformation(record);
                }

                personList.Add(person);
            }

            convertyEntity.Contractor.SharePersonList = personList;
        }

        /// <summary>
        /// 获取身份证号码
        /// </summary>
        private string SetPersonIdNumber(string personName, string identifyNumber, eCredentialsType cardType)
        {
            string idNumber = ToolString.ExceptSpaceString(identifyNumber);//身份证号码

            return idNumber.ToUpper();
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="errorInfo">错误信息</param>
        private void RecordErrorInformation(string errorInfo)
        {
            if (string.IsNullOrEmpty(errorInformation) || errorInformation.IndexOf(errorInfo) < 0)
            {
                errorInformation += errorInfo;
            }
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        public int GetAge(Person person)
        {
            if (person.CardType != eCredentialsType.IdentifyCard)
            {
                return -1;
            }
            if (!string.IsNullOrEmpty(person.ICN))
            {
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            if (!person.Birthday.HasValue)
            {
                return -1;
            }
            DateTime birthday = person.Birthday.Value;
            DateTime now = DateTime.Now;

            int year = birthday.Year;
            int nowYear = now.Year;

            int month = birthday.Month;
            int nowMonth = now.Month;

            int day = birthday.Day;
            int nowDay = now.Day;

            int age = nowYear - year;
            if (age < 1)
            {
                return -1;
            }

            return nowMonth > month || (nowMonth == month && nowDay >= day) ? age : --age;
        }
        #endregion
    }
}
