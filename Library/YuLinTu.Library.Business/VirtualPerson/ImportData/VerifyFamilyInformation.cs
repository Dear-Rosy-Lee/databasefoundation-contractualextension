/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Collections;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 校验户籍信息
    /// </summary>
    public class VerifyFamilyInformation
    {
        #region Fields

        private string errorInformation = string.Empty;//错误信息
        private string warnInformation = string.Empty;//警告信息
        private List<VirtualPerson> familys;
        private FamilyImportDefine FamilyImportDefine = FamilyImportDefine.GetIntence();
        private FamilyOtherDefine FamilyOtherDefine = FamilyOtherDefine.GetIntence();


        #endregion

        #region Properties

        /// <summary>
        /// 户籍数据
        /// </summary>
        public List<VirtualPerson> Censuses { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInformation
        {
            get { return errorInformation; }
            set { errorInformation = value; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string WarnInformation
        {
            get { return warnInformation; }
            set { warnInformation = value; }
        }


        #endregion

        #region Ctor

        public VerifyFamilyInformation(List<VirtualPerson> familys)
        {
            this.familys = familys;
        }

        #endregion

        #region 校验数据

        /// <summary>
        /// 校验户籍信息
        /// </summary>
        public bool VerifyFamily()
        {
            return CheckImportData(familys);
        }

        ///// <summary>
        ///// 校验户主与家庭成员
        ///// </summary>
        ///// <returns></returns>
        //private bool VerifyFamilyInfo()
        //{
        //    bool allRight = true;
        //    foreach (VirtualPerson family in familys)
        //    {
        //        bool isRight = false;
        //        string familyName = family.Name;//户主名
        //        family.SetFamilyPersonInfo();//设置户与人信息
        //        foreach (Person person in family.SharePersonList)
        //        {
        //            if (string.IsNullOrEmpty(person.Name))
        //                continue;
        //            if (person.Name == familyName)
        //            {
        //                isRight = true;
        //                break;
        //            }
        //        }
        //        if (!isRight)
        //        {
        //            string errorInfo = string.Format("{0}不在其家庭成员中!", familyName);
        //            RecordErrorInformation(errorInfo);//记录错误信息
        //            AddNewPerson(family);//添加人
        //            allRight = false;
        //        }
        //    }
        //    return allRight;
        //}

        ///// <summary>
        ///// 添加新人
        ///// </summary>
        ///// <param name="family"></param>
        //private void AddNewPerson(VirtualPerson family)
        //{
        //    Person person = new Person();
        //    person.ICN = family.Number;//身份证号
        //    person.Name = family.Name;//姓名
        //    person.FamilyID = family.ID;//FamilyId
        //    person.FamilyNumber = family.FamilyNumber;//户号
        //    SetPersonInformation(person, family.Number);
        //    List<Person> list = family.SharePersonList;
        //    list.Add(person);
        //    family.SharePersonList = list;
        //}

        ///// <summary>
        ///// 获取人的身份证号码
        ///// </summary>
        ///// <param name="actualPerson">实际人</param>
        ///// <param name="idNumber">身份证号码</param>
        //private void SetPersonInformation(Person person, string idNumber)
        //{
        //    if (string.IsNullOrEmpty(idNumber) || !ToolICN.Check(idNumber))
        //    {
        //        person.Gender = eGender.Unknow;
        //        person.Birthday = new DateTime(1753, 5, 4);
        //        return;
        //    }
        //    person.ICN = idNumber;
        //    person.Gender = ToolICN.GetGender(idNumber) == 1 ? eGender.Male : eGender.Female;
        //    person.Birthday = ToolICN.GetBirthday(idNumber);
        //}

        ///// <summary>
        ///// 获取所有户主名
        ///// </summary>
        ///// <returns></returns>
        //private Dictionary<string, string> GetAllPersonName()
        //{
        //    Dictionary<string, string> familyNames = new Dictionary<string, string>();
        //    foreach (VirtualPerson landInfo in familys)
        //    {
        //        familyNames.Add(landInfo.Name, landInfo.Number);
        //    }
        //    return familyNames;
        //}


        ///// <summary>
        ///// 户主名
        ///// </summary>
        //private bool VerifyAllFamilyName()
        //{
        //    bool isRight = true;
        //    for (int index = 0; index < familys.Count; index++)
        //    {
        //        VirtualPerson landInfo = familys[index];
        //        if (landInfo == null || string.IsNullOrEmpty(landInfo.Name))
        //        {
        //            continue;
        //        }
        //        bool right = CompareFamilyName(landInfo, index);//比较户主名称
        //        if (isRight)
        //        {
        //            isRight = right;
        //        }
        //    }
        //    return isRight;
        //}

        ///// <summary>
        ///// 比较户主名称
        ///// </summary>
        ///// <param name="sourceNumber"></param>
        ///// <param name="index"></param>
        ///// <param name="list"></param>
        //private bool CompareFamilyName(VirtualPerson landInfo, int index)
        //{
        //    bool isRight = true;
        //    for (int startIndex = index + 1; startIndex < familys.Count; startIndex++)
        //    {
        //        VirtualPerson comInfor = familys[startIndex];
        //        if (comInfor == null || string.IsNullOrEmpty(comInfor.Name))
        //        {
        //            continue;
        //        }
        //        if (landInfo.Name == comInfor.Name)
        //        {
        //            string warnInfo = string.Format("表中户主名{0}重复存在!", landInfo.Name);
        //            RecordWareInformation(warnInfo);
        //        }
        //        if (config.IsCheckCardNumber)
        //        {
        //            if (landInfo.Name == comInfor.Name && landInfo.Number == comInfor.Number)
        //            {
        //                string errorInfo = string.Format("表中户主名{0}重复存在,并且身份证号码相同!", landInfo.Name);
        //                RecordErrorInformation(errorInfo);
        //                isRight = false;
        //            }
        //        }
        //    }
        //    return isRight;
        //}

        ///// <summary>
        ///// 获取所有户号
        ///// </summary>
        ///// <returns></returns>
        //private ArrayList GetAllPersonIdNumber()
        //{
        //    ArrayList idNumbers = new ArrayList();
        //    foreach (VirtualPerson landInfo in familys)
        //    {
        //        foreach (Person person in landInfo.SharePersonList)
        //        {
        //            if (person != null && !string.IsNullOrEmpty(person.ICN))
        //            {
        //                idNumbers.Add(person.ICN.ToUpper());
        //            }
        //        }
        //    }
        //    return idNumbers;
        //}

        ///// <summary>
        ///// 身份证号码
        ///// </summary>
        //private void VerifyAllPersonIdNumber()
        //{
        //    ArrayList list = GetAllPersonIdNumber();//获取所有户号
        //    for (int index = 0; index < list.Count; index++)
        //    {
        //        string sourceNumber = list[index].ToString();
        //        if (string.IsNullOrEmpty(sourceNumber))
        //        {
        //            continue;
        //        }
        //        ComparePersonIdNumber(sourceNumber, index, list);
        //    }
        //}

        ///// <summary>
        ///// 比较身份证号码
        ///// </summary>
        ///// <param name="sourceNumber"></param>
        ///// <param name="index"></param>
        ///// <param name="list"></param>
        //private void ComparePersonIdNumber(string sourceNumber, int index, ArrayList list)
        //{
        //    for (int startIndex = index + 1; startIndex < list.Count; startIndex++)
        //    {
        //        string compareNumber = list[startIndex].ToString();
        //        if (string.IsNullOrEmpty(compareNumber))
        //        {
        //            continue;
        //        }
        //        if (sourceNumber == compareNumber)
        //        {
        //            string errorInfo = string.Format("表中身份证号码{0}重复存在!", sourceNumber);
        //            RecordErrorInformation(errorInfo);
        //        }
        //    }
        //}

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="errorInfo">错误信息</param>
        private void RecordErrorInformation(string errorInfo)
        {
            if (errorInfo.IsNullOrBlank())
                return;
            if (string.IsNullOrEmpty(errorInformation) || errorInformation.IndexOf(errorInfo, StringComparison.Ordinal) < 0)
            {
                errorInformation += errorInfo;
            }
        }

        ///// <summary>
        ///// 记录错误信息
        ///// </summary>
        ///// <param name="errorInfo">错误信息</param>
        //private void RecordWareInformation(string warnInfo)
        //{
        //    if (string.IsNullOrEmpty(warnInformation) || warnInformation.IndexOf(warnInfo) < 0)
        //    {
        //        warnInformation += warnInfo;
        //    }
        //}

        /// <summary>
        /// 检查导入数据
        /// </summary>
        /// <returns></returns>
        private bool CheckImportData(List<VirtualPerson> landFamilyCollection)
        {
            string info = null;
            var personList = new List<Person>();
            foreach (var family in landFamilyCollection)
            {
                family.SetFamilyPersonInfo();
                if (string.IsNullOrEmpty(family.Name))
                    continue;
                info = CheckFamilyPersonID(family, family.SharePersonList);
                RecordErrorInformation(info);
                info = CheckFamily(family, landFamilyCollection);
                RecordErrorInformation(info);
                personList.AddRange(family.SharePersonList);
            }
            foreach (var person in personList)
            {
                info = CheckSharePerson(person, personList);
                RecordErrorInformation(info);
            }
            if (string.IsNullOrEmpty(errorInformation))
                return true;
            return false;

            #region 老代码 4重foreach冒泡比较
            //bool check = true;
            //for (int i = 0; i < landFamilyCollection.Count; i++)
            //{
            //    VirtualPerson onefamily = landFamilyCollection[i];
            //    if (string.IsNullOrEmpty(onefamily.Name))
            //    {
            //        continue;
            //    }
            //    if (string.IsNullOrEmpty(onefamily.FamilyNumber))
            //    {
            //        RecordErrorInformation("承包方" + onefamily.Name + "的编号为空!");
            //        continue;
            //    }
            //    if (onefamily.SharePerson == null || onefamily.SharePersonList.Count == 0 || onefamily.SharePersonList.Find(fm => fm.Name == onefamily.Name) == null)
            //    {
            //        RecordErrorInformation("承包方" + onefamily.Name + "未包含在其家庭成员中!");
            //        continue;
            //    }
            //    int personCount = -1;
            //    int.TryParse(onefamily.PersonCount, out personCount);
            //    if (personCount != -1 && personCount != onefamily.SharePersonList.Count)
            //    {
            //        RecordErrorInformation("承包方" + onefamily.Name + "家庭成员个数与家庭成员数不一致!");
            //    }
            //    onefamily.SetFamilyPersonInfo();//设置户与人信息
            //    for (int j = i + 1; j < landFamilyCollection.Count; j++)
            //    {
            //        VirtualPerson twofamily = landFamilyCollection[j];
            //        if (onefamily.Name == twofamily.Name && string.IsNullOrEmpty(onefamily.Number))
            //        {
            //            string warnInfo = string.Format("承包方{0}在表中重复存在!", onefamily.Name);
            //            RecordErrorInformation(warnInfo);
            //        }
            //        if (onefamily.Name == twofamily.Name && onefamily.Number != twofamily.Number)
            //        {
            //            string warnInfo = string.Format("承包方{0}在表中重复存在!", onefamily.Name);
            //            RecordErrorInformation(warnInfo);
            //        }
            //        //检查身份证
            //        if (config.IsCheckCardNumber)
            //        {
            //            if (!FamilyOtherSet.NumberIcnValueRepeat && onefamily.Name == twofamily.Name &&
            //                onefamily.Number == twofamily.Number && !string.IsNullOrEmpty(onefamily.Number))
            //            {
            //                string errorInfo = string.Format("承包方{0}在表中重复存在，并且身份证号码{1}相同!", onefamily.Name,
            //                    onefamily.Number);
            //                RecordErrorInformation(errorInfo);
            //                check = false;
            //            }
            //        }
            //        if (onefamily.FamilyNumber == twofamily.FamilyNumber && !string.IsNullOrEmpty(onefamily.FamilyNumber))
            //        {
            //            int number = 0;
            //            Int32.TryParse(onefamily.FamilyNumber, out number);
            //            string errorInfo = string.Format("承包方{0}编号{1}与承包方{2}编号{3}在表中重复存在!", onefamily.Name, number, twofamily.Name, number);
            //            RecordErrorInformation(errorInfo);
            //            check = false;
            //        }
            //        List<Person> oneSharePersonList = onefamily.SharePersonList;
            //        for (int l = 0; l < oneSharePersonList.Count; l++)
            //        {
            //            Person per = oneSharePersonList[l];
            //            if (string.IsNullOrEmpty(per.ICN))
            //            {
            //                continue;
            //            }
            //            for (int k = l + 1; k < oneSharePersonList.Count; k++)
            //            {
            //                Person pe = oneSharePersonList[k];
            //                if (string.IsNullOrEmpty(pe.ICN))
            //                {
            //                    continue;
            //                }
            //                if (!config.NumberIcnValueRepeat)
            //                {
            //                    if (pe.ICN == per.ICN)
            //                    {
            //                        //string errorInfo = string.Format("家庭成员{0}身份证号码{1}与家庭成员{2}身份证号码{3}重复!", per.Name, per.ICN, pe.Name, pe.ICN);
            //                        //string errorInfo = string.Format("家庭成员{0}身份证号码{1}与家庭成员{2}身份证号码重复!", per.Name, per.ICN, pe.Name);
            //                        string errorInfo = string.Format("家庭成员{0}与{1}身份证号码{2}重复!", per.Name, pe.Name,
            //                            per.ICN);

            //                        RecordErrorInformation(errorInfo);
            //                        check = false;
            //                        return check;
            //                        //RecordErrorInformation(errorInfo);


            //                        //if (AgricultureSetting.AllowIdentifyNumberRepeat)
            //                        //{
            //                        //    RecordWareInformation(errorInfo);
            //                        //}
            //                        //if (FamilyOtherSet.NumberIcnValueRepeat)
            //                        //{
            //                        //    RecordWareInformation(errorInfo);
            //                        //}
            //                        //else
            //                        //{
            //                        //    RecordErrorInformation(errorInfo);
            //                        //    check = false;
            //                        //}

            //                    }
            //                }
            //            }
            //        }
            //        List<Person> twoSharePersonList = twofamily.SharePersonList;
            //        foreach (Person p1 in oneSharePersonList)
            //        {
            //            if (string.IsNullOrEmpty(p1.ICN))
            //            {
            //                continue;
            //            }
            //            foreach (Person p2 in twoSharePersonList)
            //            {
            //                if (string.IsNullOrEmpty(p2.ICN))
            //                {
            //                    continue;
            //                }
            //                if (config.IsCheckCardNumber)
            //                {
            //                    if (p1.ICN == p2.ICN)
            //                    {
            //                        //string errorInfo = string.Format("{0}家庭成员{1}身份证号码{2}与{3}家庭成员{4}身份证号码{5}重复!",
            //                        //    onefamily.Name, p1.Name, p1.ICN, twofamily.Name, p2.Name, p2.ICN);
            //                        //string errorInfo = string.Format("{0}家庭成员{1}身份证号码{2}与{3}家庭成员{4}身份证号码重复!",
            //                        //    onefamily.Name, p1.Name, p1.ICN, twofamily.Name, p2.Name);
            //                        string errorInfo = string.Format("{0}家庭成员{1}与{2}家庭成员{3}身份证号码{4}重复!",
            //                          onefamily.Name, p1.Name, twofamily.Name, p2.Name, p1.ICN);
            //                        //if (AgricultureSetting.AllowIdentifyNumberRepeat)
            //                        //{
            //                        //    RecordWareInformation(errorInfo);
            //                        //}
            //                        if (FamilyOtherSet.NumberIcnValueRepeat)
            //                        {
            //                            RecordWareInformation(errorInfo);
            //                        }
            //                        else
            //                        {
            //                            RecordErrorInformation(errorInfo);
            //                            check = false;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //return check;
            #endregion

        }


        /// <summary>
        /// 检查一户人中身份证号码是否重复
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        private string CheckFamilyPersonID(VirtualPerson vp, List<Person> personList)
        {
            string result = null;
            if (FamilyImportDefine.NumberIcnIndex == -1) return result;//如果不导入身份证则验证通过
            //在一户人中身份证号码一定不能相同
            foreach (var person in personList)
            {
                if (FamilyOtherDefine.NumberIcnValueRepeat)
                {
                    if (personList.Any(o => o.Name == person.Name && string.IsNullOrWhiteSpace(o.ICN) && string.IsNullOrWhiteSpace(person.ICN) && o.ID != person.ID))
                        result += ("共有人:" + person.Name + ",在承包方:" + vp.Name + "户下姓名重复,且证件号码为空！");
                }
                /* 修改于2016/8/31 判断排除本人 */
                if (personList.Any(o => o.Name == person.Name && o.ICN == person.ICN && o.ID != person.ID))
                    result += ("共有人:" + person.Name + ",在承包方:" + vp.Name + "户下姓名重复，且身份证号码相同！");
            }
            return result;
        }

        /// <summary>
        /// 检验户主信息
        /// </summary>
        /// <param name="person"></param>
        /// <param name="landFamilyCollection"></param>
        /// <returns></returns>
        private string CheckFamily(VirtualPerson person, List<VirtualPerson> landFamilyCollection)
        {
            string result = null;
            if (string.IsNullOrEmpty(person.FamilyNumber))
                result += "承包方" + person.Name + "的编号为空!";
            if (person.SharePerson == null || person.SharePersonList.Count == 0 || person.SharePersonList.Find(fm => fm.Name == person.Name) == null)
                result += "承包方" + person.Name + "未包含在其家庭成员中!";
            int personCount;
            int.TryParse(person.PersonCount, out personCount);
            if (personCount != -1 && personCount != person.SharePersonList.Count)
                result += "承包方" + person.Name + "家庭成员个数与家庭成员数不一致!";
            if (landFamilyCollection.Any(o => o.FamilyNumber == person.FamilyNumber && o.ID != person.ID))
                result += ("承包方编号" + person.Name + "在表中重复存在!");
            if (FamilyImportDefine.NumberIcnIndex == -1) return result;//如果不导入身份证则验证通过

            if (FamilyOtherDefine.NumberIcnValueRepeat)
            {
                if (landFamilyCollection.Any(o => o.Name == person.Name &&
                    string.IsNullOrWhiteSpace(o.Number) && string.IsNullOrWhiteSpace(person.Number)
                    && o.ID != person.ID))
                    result += ("承包方" + person.Name + "在表中姓名重复，且证件为空!");
            }
            if (landFamilyCollection.Any(o => o.Number == person.Number && o.Name == person.Name && o.ID != person.ID))
                result += ("承包方" + person.Name + "在表中重复，且证件号码相同!");

            return result;
        }

        /// <summary>
        /// 校验共有人身份证号
        /// </summary>
        /// <param name="person"></param>
        /// <param name="personList"></param>
        /// <returns></returns>
        private string CheckSharePerson(Person person, List<Person> personList)
        {
            string result = null;
            if (FamilyImportDefine.NumberIcnIndex == -1) return result;//如果不导入身份证则验证通过
            if (FamilyOtherDefine.IsPromiseCardNumberNullEnable) //允许身份证为空
            {
                if (string.IsNullOrWhiteSpace(person.ICN))
                    return result;
            }
            if (FamilyOtherDefine.IsCheckCardNumber)
            {
                if (person.CardType==eCredentialsType.IdentifyCard && !ToolICN.Check(person.ICN))
                    result += ("承包方共有人" + person.Name + "身份证号码不正确!");
            }
            if (FamilyOtherDefine.NumberIcnValueRepeat) return result;
            if (personList.Any(o => o.ICN == person.ICN  && o.ID != person.ID))//&& o.Name == person.Name
            {
                result += ("承包方共有人" + person.Name + "证件号码重复!");
            }
            return result;
        }

        #endregion
    }
}
