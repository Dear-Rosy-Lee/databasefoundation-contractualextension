/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方处理类
    /// </summary>
    public class VirtualPersonOperator
    {
        #region Static

        public const string ISAGRICULTUREBUSINESSREGISTER = "IsAgricultureBusinessRegister";//是否是农用地业务已登记操作
        public const string ISUSESHAREPERSONVALUE = "IsUseSharePersonValue";//是否只使用共有人

        /// <summary>
        /// 是否是农用地业务已登记操作
        /// </summary>
        public static bool IsAgricultureBusinessRegister
        {
            get
            {
                bool success = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(ISAGRICULTUREBUSINESSREGISTER, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(ISAGRICULTUREBUSINESSREGISTER, value.ToString());
            }
        }

        /// <summary>
        /// 空字符串处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NullStringPergress(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "空";
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(value)))
            {
                return "空";
            }
            return value;
        }

        /// <summary>
        /// 是否只使用共有人
        /// </summary>
        public static bool IsUseSharePersonValue
        {
            get
            {
                bool success = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, value.ToString());
            }
        }

        /// <summary>
        /// 初始化日期字符串
        /// </summary>
        /// <returns></returns>
        public static string InitalizeDateString(DateTime? date)
        {
            if (date == null || !date.HasValue)
            {
                return "";
            }
            return ToolDateTime.GetLongDateString(date.Value);
        }


        /// <summary>
        /// 初始化名称
        /// </summary>
        public static string InitalizeFamilyName(string name, bool except = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            if (!except)
            {
                return name;
            }
            string number = ToolString.GetAllNumberWithInString(name);
            if (!string.IsNullOrEmpty(number))
            {
                return name.Replace(number, "");
            }
            int index = name.IndexOf("(");
            if (index > 0)
            {
                return name.Substring(0, index);
            }
            index = name.IndexOf("（");
            if (index > 0)
            {
                return name.Substring(0, index);
            }
            return name;
        }

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="family">户主</param>
        /// <returns></returns>
        public static bool Equal(VirtualPerson family, VirtualPerson virtualperson)
        {
            string postNumber = string.IsNullOrEmpty(family.PostalNumber) ? "" : family.PostalNumber;
            string comment = string.IsNullOrEmpty(family.Comment) ? "" : family.Comment;
            string sourcePostNumber = string.IsNullOrEmpty(virtualperson.PostalNumber) ? "" : virtualperson.PostalNumber;
            string sourceComment = string.IsNullOrEmpty(virtualperson.Comment) ? "" : virtualperson.Comment;
            string sourcePersonCount = string.IsNullOrEmpty(virtualperson.PersonCount) ? "" : virtualperson.PersonCount;
            string personCount = string.IsNullOrEmpty(family.PersonCount) ? "" : family.PersonCount;
            bool isEqual = virtualperson.Name == family.Name
                        && virtualperson.CardType == family.CardType
                        && virtualperson.Number == family.Number
                        && virtualperson.ZoneCode == family.ZoneCode
                        && virtualperson.Telephone == family.Telephone
                        && virtualperson.FamilyNumber == family.FamilyNumber
                        && sourcePostNumber == postNumber
                        && sourceComment == comment
                        && sourcePersonCount == personCount;
            return isEqual;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="family"></param>
        /// <returns></returns>
        public static string Compare(VirtualPerson family, VirtualPerson virtualPerson)
        {
            string informtion = "户主" + family.Name;
            string postNumber = string.IsNullOrEmpty(family.PostalNumber) ? "" : family.PostalNumber;
            string comment = string.IsNullOrEmpty(family.Comment) ? "" : family.Comment;
            string sourcePostNumber = string.IsNullOrEmpty(virtualPerson.PostalNumber) ? "" : virtualPerson.PostalNumber;
            string sourceComment = string.IsNullOrEmpty(virtualPerson.Comment) ? "" : virtualPerson.Comment;
            string sourcePersonCount = string.IsNullOrEmpty(virtualPerson.PersonCount) ? "" : virtualPerson.PersonCount;
            string personCount = string.IsNullOrEmpty(family.PersonCount) ? "" : family.PersonCount;
            string address = string.IsNullOrEmpty(family.Address) ? "" : family.Address;
            if (virtualPerson.Name != family.Name)
            {
                informtion += "名称由" + NullStringPergress(virtualPerson.Name) + "更改为" + NullStringPergress(family.Name) + ";";
            }
            if (virtualPerson.CardType != family.CardType && virtualPerson.CardType != 0 && family.CardType != 0)
            {
                informtion += "证件类型由" + EnumNameAttribute.GetDescription(virtualPerson.CardType) + "更改为" + EnumNameAttribute.GetDescription(family.CardType) + ";";
            }
            if (virtualPerson.Number != family.Number)
            {
                informtion += "证件号码由" + NullStringPergress(virtualPerson.Number) + "更改为" + NullStringPergress(family.Number) + ";";
            }
            if (virtualPerson.Telephone != family.Telephone)
            {
                informtion += "电话号码由" + NullStringPergress(virtualPerson.Telephone) + "更改为" + NullStringPergress(family.Telephone) + ";";
            }
            if (virtualPerson.FamilyNumber != family.FamilyNumber)
            {
                informtion += "户号由" + NullStringPergress(virtualPerson.FamilyNumber) + "更改为" + NullStringPergress(family.FamilyNumber) + ";";
            }
            if (sourcePostNumber != postNumber)
            {
                informtion += "邮政编码由" + NullStringPergress(sourcePostNumber) + "更改为" + NullStringPergress(postNumber) + ";";
            }
            if (virtualPerson.Address != address)
            {
                informtion += "承包方地址由" + NullStringPergress(virtualPerson.Address) + "更改为" + NullStringPergress(address) + ";";
            }
            VirtualPersonExpand sourceExpand = virtualPerson.FamilyExpand;
            VirtualPersonExpand targetExpand = family.FamilyExpand;
            if (sourceExpand.SurveyPerson != targetExpand.SurveyPerson)
            {
                informtion += "承包方调查员由" + NullStringPergress(sourceExpand.SurveyPerson) + "更改为" + NullStringPergress(targetExpand.SurveyPerson) + ";";
            }
            if (sourceExpand.SurveyDate != targetExpand.SurveyDate)
            {
                informtion += "承包方调查日期由" + InitalizeDateString(sourceExpand.SurveyDate) + "更改为" + InitalizeDateString(targetExpand.SurveyDate) + ";";
            }
            if (sourceExpand.SurveyChronicle != targetExpand.SurveyChronicle)
            {
                informtion += "承包方调查记事由" + NullStringPergress(sourceExpand.SurveyChronicle) + "更改为" + NullStringPergress(targetExpand.SurveyChronicle) + ";";
            }
            if (sourceExpand.CheckPerson != targetExpand.CheckPerson)
            {
                informtion += "承包方审核人由" + NullStringPergress(sourceExpand.CheckPerson) + "更改为" + NullStringPergress(targetExpand.CheckPerson) + ";";
            }
            if (sourceExpand.CheckDate != targetExpand.CheckDate)
            {
                informtion += "承包方审核日期由" + InitalizeDateString(sourceExpand.CheckDate) + "更改为" + InitalizeDateString(targetExpand.CheckDate) + ";";
            }
            if (sourceExpand.CheckOpinion != targetExpand.CheckOpinion)
            {
                informtion += "承包方审核意见由" + NullStringPergress(sourceExpand.CheckOpinion) + "更改为" + NullStringPergress(targetExpand.CheckOpinion) + ";";
            }
            if (sourceExpand.PublicityChroniclePerson != targetExpand.PublicityChroniclePerson)
            {
                informtion += "承包方公示记事人由" + NullStringPergress(sourceExpand.PublicityChroniclePerson) + "更改为" + NullStringPergress(targetExpand.PublicityChroniclePerson) + ";";
            }
            if (sourceExpand.PublicityDate != targetExpand.PublicityDate)
            {
                informtion += "承包方公示日期由" + InitalizeDateString(sourceExpand.PublicityDate) + "更改为" + InitalizeDateString(targetExpand.PublicityDate) + ";";
            }
            if (sourceExpand.PublicityChronicle != targetExpand.PublicityChronicle)
            {
                informtion += "承包方公示记事由" + NullStringPergress(sourceExpand.PublicityChronicle) + "更改为" + NullStringPergress(targetExpand.PublicityChronicle) + ";";
            }
            if (sourceExpand.PublicityCheckPerson != targetExpand.PublicityCheckPerson)
            {
                informtion += "承包方公示审核人由" + NullStringPergress(sourceExpand.PublicityCheckPerson) + "更改为" + NullStringPergress(targetExpand.PublicityCheckPerson) + ";";
            }
            if (sourceExpand.ConstructMode != targetExpand.ConstructMode)
            {
                informtion += "承包方取得承包方式由" + EnumNameAttribute.GetDescription(sourceExpand.ConstructMode) + "更改为" + EnumNameAttribute.GetDescription(targetExpand.ConstructMode) + ";";
            }
            if (sourceExpand.ConcordNumber != targetExpand.ConcordNumber)
            {
                informtion += "承包方承包合同编号由" + NullStringPergress(sourceExpand.ConcordNumber) + "更改为" + NullStringPergress(targetExpand.ConcordNumber) + ";";
            }
            if (sourceExpand.WarrantNumber != targetExpand.WarrantNumber)
            {
                informtion += "承包方经营权证编号由" + NullStringPergress(sourceExpand.WarrantNumber) + "更改为" + NullStringPergress(targetExpand.WarrantNumber) + ";";
            }
            if (sourceExpand.ConcordStartTime != targetExpand.ConcordStartTime)
            {
                informtion += "承包方承包起始日期由" + InitalizeDateString(sourceExpand.ConcordStartTime) + "更改为" + InitalizeDateString(targetExpand.ConcordStartTime) + ";";
            }
            if (sourceExpand.ConcordEndTime != targetExpand.ConcordEndTime)
            {
                informtion += "承包方承包结束日期由" + InitalizeDateString(sourceExpand.ConcordEndTime) + "更改为" + InitalizeDateString(targetExpand.ConcordEndTime) + ";";
            }
            if (sourceComment != comment)
            {
                informtion += "备注由" + NullStringPergress(sourceComment) + "更改为" + NullStringPergress(comment) + ";";
            }
            if (informtion == "户主" + family.Name)
            {
                informtion = "";
            }
            return informtion;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static string Compare(Person person, Person p)
        {
            string information = person.Name + "的";
            if (p.Name != person.Name)
            {
                information += "名称由" + NullStringPergress(p.Name) + "更改为" + NullStringPergress(person.Name) + ";";
            }
            if (p.CardType != person.CardType)
            {
                information += "证件类型由" + EnumNameAttribute.GetDescription(p.CardType) + "更改为" + EnumNameAttribute.GetDescription(person.CardType) + ";";
            }
            if (p.ICN != person.ICN)
            {
                information += "证件号码由" + NullStringPergress(p.ICN) + "更改为" + NullStringPergress(person.ICN) + ";";
            }
            if (p.IsSharedLand != person.IsSharedLand)
            {
                information += "是否共有人由" + NullStringPergress(p.IsSharedLand) + "更改为" + NullStringPergress(person.IsSharedLand) + ";";
            }
            if (p.Gender != person.Gender)
            {
                information += "性别由" + EnumNameAttribute.GetDescription(p.Gender) + "更改为" + EnumNameAttribute.GetDescription(person.Gender) + ";";
            }
            if (p.Nation != person.Nation)
            {
                information += "民族由" + EnumNameAttribute.GetDescription(p.Nation) + "更改为" + EnumNameAttribute.GetDescription(person.Nation) + ";";
            }
            if (p.Relationship != person.Relationship)
            {
                information += "家庭关系由" + NullStringPergress(p.Relationship) + "更改为" + NullStringPergress(person.Relationship) + ";";
            }
            return information;
        }

        #endregion
    }
}
