/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 读取户籍表
    /// </summary>
    public class ReadFamilyInformation : ExcelBase
    {
        #region Fields

        private List<VirtualPerson> familys = new List<VirtualPerson>();//所有表数据集合
        private List<VirtualPerson> tableFamilys = new List<VirtualPerson>();//所有表数据集合
        private string fileName = string.Empty;//文件名
        private string lastRowOneText = "合计";//最后一行第一个单元格中文字
        private string errorInformation = string.Empty;//错误信息
        private int currentIndex = 0;//当前行数
        private int currentNumber = 0;//当前编号
        private FamilyImportDefine familyDefine;
        //private FamilyOtherDefine otherDefine;
        private int columnCount;//列数

        #endregion

        #region Propertys

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        public FamilyImportDefine FamilyImportSet
        {
            get { return familyDefine; }
            set { familyDefine = value; }
        }

        ///// <summary>
        ///// 承包方基本配置
        ///// </summary>
        //public FamilyOtherDefine OtherDefine
        //{
        //    get { return otherDefine; }
        //    set { otherDefine = value; }
        //}

        /// <summary>
        /// 类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 所有表信息
        /// </summary>
        public List<VirtualPerson> ExcelFamilys
        {
            get { return familys; }
        }

        /// <summary>
        /// 所有表信息
        /// </summary>
        public List<VirtualPerson> TableFamilys
        {
            get { return tableFamilys; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInformation
        {
            get { return errorInformation; }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string PromptInformation { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public ReadFamilyInformation(string fileName)
        {
            this.fileName = fileName;//文件名
            InitalizeInnerData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeInnerData()
        {
            //string filePath = Application.StartupPath + @"\Config\" + "FamilyImportDefine.xml";
            //if (File.Exists(filePath))
            //{
            //    familyDefine = ToolSerialization.DeserializeXml(filePath, typeof(FamilyImportDefine)) as FamilyImportDefine;
            //}
            if (familyDefine == null)
            {
                familyDefine = new FamilyImportDefine();
            }
        }

        #endregion

        #region Method
        /// <summary>
        /// 读方法
        /// </summary>
        public override void Read()
        {
        }

        /// <summary>
        /// 写方法
        /// </summary>
        public override void Write()
        {
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Disponse()
        {
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public bool OpenFamilyFile()
        {
            try
            {
                Open(this.fileName);//打开文件
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 读取数据

        /// <summary>
        /// 获取开始行数据
        /// </summary>
        /// <param name="range">值范围</param>
        /// <param name="allItem">所有值</param>
        /// <returns></returns>
        private int GetStartRow(int rowCount, object[,] allItem)
        {
            int startIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                string rowValue = GetString(allItem[i, 0]);//编号栏数据
                if (!string.IsNullOrEmpty(rowValue))
                {
                    int.TryParse(rowValue, out startIndex);
                }
                if (startIndex != 0)
                {
                    startIndex = i;
                    break;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public void ReadInformation(bool isStockRight)
        {
            object[,] allItem = GetAllRangeValue();//获取所有使用域值
            if (allItem == null)
            {
                RecordErrorInformation("表中可能没有数据或数据已损坏,若表中有数据则重新建表,并将原数据拷贝过去,再执行该操作!");
                return;
            }
            VirtualPerson family = new VirtualPerson();
            //VirtualPerson table = new VirtualPerson();
            string totalInfo = string.Empty;
            int rowCount = GetRangeRowCount();
            int calIndex = GetStartRow(rowCount, allItem);//获取数据开始行数
            columnCount = GetRangeColumnCount();
            for (int index = calIndex; index < rowCount; index++)
            {
                currentIndex = index;//当前行数 
                string rowValue = GetString(allItem[index, 0]);//编号栏数据
                if (rowValue.Trim() == lastRowOneText || rowValue.Trim() == "总计" || rowValue.Trim() == "共计")
                {
                    break;
                }
                string familyName = familyDefine.NameIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NameIndex]) : "";
                if (!string.IsNullOrEmpty(rowValue) || !string.IsNullOrEmpty(familyName))
                {
                    int.TryParse(rowValue, out currentNumber);//当前编号
                    family = new VirtualPerson();
                    if (!string.IsNullOrEmpty(rowValue))
                    {
                        family.FamilyNumber = rowValue;
                        if (!ToolMath.MatchEntiretyNumber(rowValue))
                        {
                            string errorInformation = string.Format("表中第{0}行承包方编号{1}不符合数字类型要求!", currentIndex + 1, rowValue);
                            RecordErrorInformation(errorInformation);
                        }
                    }
                    if (!string.IsNullOrEmpty(familyName))
                    {
                        family.Name = familyName;
                    }
                }
                try
                {
                    GetExcelInformation(family, allItem,isStockRight);//获取Excel表中信息
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "GetExcelInformation(获取Excel表中信息)", ex.Message + ex.StackTrace);
                    throw new Exception("获取表格信息失败: 配置索引项大于表格索引项。" + "请检查表格格式是否正确后再执行导入操作!");
                }

            }
            allItem = null;
            rowCount = 0;
            currentIndex = 0;
            GC.Collect();
        }

        /// <summary>
        /// 获取Excel表中信息
        /// </summary>
        /// <param name="allItem">所有项目</param>
        private void GetExcelInformation(VirtualPerson family, object[,] allItem,bool isStockRight)
        {
            string familyName = familyDefine.NameIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NameIndex]) : "";
            if (!string.IsNullOrEmpty(familyName))
            {
                SetLandFamilyInfo(family, familyName, allItem,isStockRight);//设置户信息
            }
            string personName = familyDefine.NumberNameIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberNameIndex]) : "";//家庭成员名
            if (family != null && !string.IsNullOrEmpty(family.Name) && !string.IsNullOrEmpty(personName))//添加人
            {
                SetLandPersonInfo(family, personName, allItem);//设置家庭成员信息
            }
            else
            {
                if (!string.IsNullOrEmpty(personName))
                {
                    string errorInfo = string.Format("序号为{0}的户主姓名为空!", currentNumber);
                    RecordErrorInformation(errorInfo);//记录错误信息
                }
            }
            //处理二轮延包家庭成员信息
            //string tablePersonName = familyDefine.ExPackageNameIndex > 0 ? GetString(allItem[currentIndex, familyDefine.ExPackageNameIndex]) : "";//家庭成员名
            //if (family != null && !string.IsNullOrEmpty(family.Name) && !string.IsNullOrEmpty(tablePersonName))//添加人
            //{
            //    //VirtualPerson tableVp = family.Clone() as VirtualPerson;
            //    //tableVp.SharePersonList.Clear();
            //    SetTablePersonInfo(family, tablePersonName, allItem);//设置家庭成员信息
            //}
        }

        ///// <summary>
        ///// 获取二轮信息
        ///// </summary>
        ///// <param name="allItem">所有项目</param>
        //private void GetTableInformation(VirtualPerson family, object[,] allItem)
        //{
        //    //处理二轮延包家庭成员信息
        //    string tablePersonName = familyDefine.ExPackageNameIndex > 0 ? GetString(allItem[currentIndex, familyDefine.ExPackageNameIndex]) : "";//家庭成员名
        //    if (family != null && !string.IsNullOrEmpty(family.Name) && !string.IsNullOrEmpty(tablePersonName))//添加人
        //    {
        //        VirtualPerson tableVp = family.Clone() as VirtualPerson;
        //        tableVp.SharePersonList.Clear();
        //        SetTablePersonInfo(tableVp, tablePersonName, allItem);//设置家庭成员信息
        //    }
        //}

        /// <summary>
        /// 设置家庭信息
        /// </summary>
        /// <param name="family">户对象</param>
        /// <param name="familyName">户主名</param>
        /// <param name="allItem">所有值</param>
        private void SetLandFamilyInfo(VirtualPerson curFamily, string familyName, object[,] allItem,bool isStockRight)
        {
            //Family family = new Family();
            //family.HouseholderName = familyName;//户主名
            //family.Name = GetString(familyName);//姓名
            //family.FamilyNumber = curFamily.CurrentFamily.FamilyNumber;
            //curFamily.CurrentFamily = family;
            curFamily.Name = familyName;
            string value = familyDefine.NumberIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberIndex]) : "";
            int count = -1;
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, out count);
            }
            if (count == 0)
            {
                RecordErrorInformation(string.Format("表中{0}的家庭成员个数{1}填写无效,不是有效数字!", familyName, value));
            }
            if (count == -1 && familyDefine.NumberIndex >= 0)
            {
                RecordErrorInformation(string.Format("表中{0}的家庭成员个数未填写内容!", familyName));
            }
            curFamily.PersonCount = count.ToString();
            curFamily.Address = familyDefine.ContractorAddressIndex > 0 ? GetString(allItem[currentIndex, familyDefine.ContractorAddressIndex]) : "";
            curFamily.PostalNumber = familyDefine.PostNumberIndex > 0 ? GetString(allItem[currentIndex, familyDefine.PostNumberIndex]) : "";
            if (!string.IsNullOrEmpty(curFamily.PostalNumber) && (!ToolMath.MatchEntiretyNumber(curFamily.PostalNumber) || curFamily.PostalNumber.Length != 6))
            {
                RecordErrorInformation(string.Format("表中{0}的邮政编码{1}不满足6位数字要求!", familyName, curFamily.PostalNumber));
            }
            curFamily.Telephone = familyDefine.TelephoneIndex > 0 ? GetString(allItem[currentIndex, familyDefine.TelephoneIndex]) : "";
            if (!string.IsNullOrEmpty(curFamily.Telephone) && !ToolMath.MatchEntiretyNumber(curFamily.Telephone.Replace("-", "")))
            {
                RecordErrorInformation(string.Format("表中{0}的联系电话{1}不满足数字要求!", familyName, curFamily.PostalNumber));
            }
            VirtualPersonExpand expand = new VirtualPersonExpand();
            expand.HouseHolderName = familyName;
            expand.SurveyPerson = familyDefine.SurveyPersonIndex > 0 ? GetString(allItem[currentIndex, familyDefine.SurveyPersonIndex]) : "";
            string cellValue = familyDefine.SurveyDateIndex > 0 ? GetString(allItem[currentIndex, familyDefine.SurveyDateIndex]) : "";
            expand.SurveyDate = familyDefine.SurveyDateIndex > 0 ? GetDateTime(allItem[currentIndex, familyDefine.SurveyDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (expand.SurveyDate == null || !expand.SurveyDate.HasValue))
            {
                string information = string.Format("表中{0}的承包方调查日期{1}不符合日期填写要求!", expand.HouseHolderName, cellValue);
                RecordErrorInformation(information);
            }
            expand.SurveyChronicle = familyDefine.SurveyChronicleIndex > 0 ? GetString(allItem[currentIndex, familyDefine.SurveyChronicleIndex]) : "";
            expand.CheckPerson = familyDefine.CheckPersonIndex > 0 ? GetString(allItem[currentIndex, familyDefine.CheckPersonIndex]) : "";
            cellValue = familyDefine.CheckDateIndex > 0 ? GetString(allItem[currentIndex, familyDefine.CheckDateIndex]) : "";
            expand.CheckDate = familyDefine.CheckDateIndex > 0 ? GetDateTime(allItem[currentIndex, familyDefine.CheckDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (expand.CheckDate == null || !expand.CheckDate.HasValue))
            {
                string information = string.Format("表中{0}的承包方审核日期{1}不符合日期填写要求!", expand.HouseHolderName, cellValue);
                RecordErrorInformation(information);
            }
            expand.CheckOpinion = familyDefine.CheckOpinionIndex > 0 ? GetString(allItem[currentIndex, familyDefine.CheckOpinionIndex]) : "";
            expand.AllocationPerson = familyDefine.AllocationPersonIndex > 0 ? GetString(allItem[currentIndex, familyDefine.AllocationPersonIndex]) : "";
            expand.ConcordNumber = familyDefine.SecondConcordNumberIndex > 0 ? GetString(allItem[currentIndex, familyDefine.SecondConcordNumberIndex]) : "";
            expand.WarrantNumber = familyDefine.SecondWarrantNumberIndex > 0 ? GetString(allItem[currentIndex, familyDefine.SecondWarrantNumberIndex]) : "";
            cellValue = familyDefine.StartTimeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.StartTimeIndex]) : "";
            expand.ConcordStartTime = familyDefine.StartTimeIndex > 0 ? GetDateTime(allItem[currentIndex, familyDefine.StartTimeIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (expand.ConcordStartTime == null || !expand.ConcordStartTime.HasValue))
            {
                string information = string.Format("表中{0}的承包方合同开始日期{1}不符合日期填写要求!", expand.HouseHolderName, cellValue);
                RecordErrorInformation(information);
            }
            cellValue = familyDefine.EndTimeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.EndTimeIndex]) : "";
            expand.ConcordEndTime = familyDefine.EndTimeIndex > 0 ? GetDateTime(allItem[currentIndex, familyDefine.EndTimeIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (expand.ConcordEndTime == null || !expand.ConcordEndTime.HasValue))
            {
                string information = string.Format("表中{0}的承包方合同结束日期{1}不符合日期填写要求!", expand.HouseHolderName, cellValue);
                RecordErrorInformation(information);
            }
            string constructType = familyDefine.ConstructTypeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.ConstructTypeIndex]) : "";
            if (!string.IsNullOrEmpty(constructType))
                expand.ConstructMode = GetConstructMode(constructType);
            else
                expand.ConstructMode = eConstructMode.Family;
            expand.EquityValue = familyDefine.EquityNumberIndex > 0 ? GetDouble(allItem[currentIndex, familyDefine.EquityNumberIndex]) : 0.0;
            expand.EquityArea = familyDefine.EquityAreaIndex > 0 ? GetDouble(allItem[currentIndex, familyDefine.EquityAreaIndex]) : 0.0;
            expand.SecondConcordTotalArea = familyDefine.SecondConcordTotalArea > 0 ? GetDouble(allItem[currentIndex, familyDefine.SecondConcordTotalArea]) : 0.0;
            expand.SecondConcordTotalLandCount = familyDefine.SecondConcordTotalLandCount > 0 ? GetInt32(allItem[currentIndex, familyDefine.SecondConcordTotalLandCount]) : 0;
            expand.ExtendName = familyDefine.ExtendName > 0 ? GetString(allItem[currentIndex, familyDefine.ExtendName]) : "";
            InitalizeFamilyType(familyName, allItem, expand);
            curFamily.FamilyExpand = expand;
            curFamily.IsStockFarmer = isStockRight;
            familys.Add(curFamily);
            ReportSurveyDateInformation(expand);
            ReportSurveyConcordDateInformation(expand);
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
            if (familyDefine.SurveyDateIndex < 0 || familyDefine.CheckDateIndex < 0)
            {
                return;
            }
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
            if (familyDefine.StartTimeIndex < 0 || familyDefine.EndTimeIndex < 0)
            {
                return;
            }
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
        private void InitalizeFamilyType(string familyName, object[,] allItem, VirtualPersonExpand expand)
        {
            if (familyDefine.ContractorTypeIndex < 0)
            {
                expand.ContractorType = eContractorType.Farmer;
                return;
            }
            string value = familyDefine.ContractorTypeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.ContractorTypeIndex]) : "";
            string errorInformation = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                errorInformation = string.Format("表中{0}的承包方类型未填写信息!", familyName);
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
                    errorInformation = string.Format("表中{0}的承包方类型{1}填写错误,不是{2}其中一种!", familyName, value, InitalizeEnumDescription(typeof(eContractorType)));
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
        private void SetLandPersonInfo(VirtualPerson family, string personName, object[,] allItem)
        {
            Person person = new Person() { };
            person.Name = personName;//人名

            string identifyNumber = familyDefine.NumberIcnIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberIcnIndex]) : "";//身份证号码
            if (TableType == 2)
            {
                identifyNumber = GetString(allItem[currentIndex, 6]);//身份证号码
                string tableArea = GetString(allItem[currentIndex, 7]);
                if (!string.IsNullOrEmpty(tableArea))
                {
                    family.Comment = "二轮台账面积:" + tableArea + "亩";//二轮台账面积
                }
            }
            string objValue = familyDefine.NumberCartTypeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberCartTypeIndex]) : "";
            string errorInformation = string.Empty;
            if (familyDefine.NumberCartTypeIndex > 0)
            {
                if (string.IsNullOrEmpty(objValue))
                {
                    errorInformation = string.Format("表中{0}的证件类型未填写信息!", personName);
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
                        errorInformation = string.Format("表中{0}的证件类型{1}填写错误,不是{2}其中一种!", personName, objValue, InitalizeEnumDescription(typeof(eCredentialsType)));
                        RecordErrorInformation(errorInformation);
                    }
                }
            }
            else
            {
                person.CardType = eCredentialsType.IdentifyCard;
            }
            person.ICN = SetPersonIdNumber(personName, identifyNumber, person.CardType);//设置身份证号码
            person.Gender = eGender.Unknow;//2011-3-25修改
            if (ToolICN.Check(person.ICN) && person.CardType == eCredentialsType.IdentifyCard)
            {
                person.Gender = ToolICN.GetGender(person.ICN) == 1 ? eGender.Male : eGender.Female;
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            else
            {
                //2011-3-25 修改人 魏巍  3.14需求，只要身份证号长度是15或18就去读性别生日
                //if (!string.IsNullOrEmpty(person.ICN) && (person.ICN.Length == 15 || person.ICN.Length == 18))
                //{
                //    person.Gender = ToolICN.GetGenderInNotCheck(person.ICN) == 1 ? eGender.Male : eGender.Female;
                //    person.Birthday = ToolICN.GetBirthdayInNotCheck(person.ICN);
                //}
                //else
                //{
                    person.Gender = eGender.Unknow;
                    string ageValue = familyDefine.NumberAgeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberAgeIndex]) : "";
                    int age = 0;
                    int.TryParse(ageValue, out age);
                    if (age > 0)
                    {
                        person.Birthday = new DateTime(DateTime.Now.Year - age, 1, 1);
                    }
                    else
                    {
                        //person.Birthday = new DateTime(1753, 5, 4);
                        person.Birthday = null;
                    }
                //}
            }
            person.Nation = eNation.Han;
            if (person.Gender == eGender.Unknow)
            {
                string value = familyDefine.NumberGenderIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberGenderIndex]) : "";
                if (value == "男")
                {
                    person.Gender = eGender.Male;
                }
                else if (value == "女")
                {
                    person.Gender = eGender.Female;
                }
            }
            try
            {
                if (!person.Birthday.HasValue || person.Birthday.Value.Month == 1753)
                {
                    string ageValue = familyDefine.NumberAgeIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberAgeIndex]) : "";
                    int age = 0;
                    int.TryParse(ageValue, out age);
                    if (age > 0)
                    {
                        person.Birthday = new DateTime(DateTime.Now.Year - age, 1, 1);
                    }
                }
                person.Relationship = familyDefine.NumberRelatioinIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NumberRelatioinIndex]) : "";
            }
            catch { }
            person.FamilyID = family.ID;
            string nation = familyDefine.NationIndex > 0 ? GetString(allItem[currentIndex, familyDefine.NationIndex]) : "";
            if (!string.IsNullOrEmpty(nation) && nation.Substring(nation.Length - 1) != "族")
            {
                nation += "族";
            }
            object obj = EnumNameAttribute.GetValue(typeof(eNation), nation);
            if (obj != null)
            {
                person.Nation = (eNation)EnumNameAttribute.GetValue(typeof(eNation), nation);
            }
            person.AccountNature = familyDefine.AccountNatureIndex > 0 ? GetString(allItem[currentIndex, familyDefine.AccountNatureIndex]) : "";
            person.Comment = familyDefine.CommentIndex > 0 ? GetString(allItem[currentIndex, familyDefine.CommentIndex]) : "";
            person.Opinion = familyDefine.OpinionIndex > 0 ? GetString(allItem[currentIndex, familyDefine.OpinionIndex]) : "";
            person.CencueComment = familyDefine.CencueCommentIndex > 0 ? GetString(allItem[currentIndex, familyDefine.CencueCommentIndex]) : "";
            if (familyDefine.IsSharedLandIndex > 0)
            {
                person.IsSharedLand = familyDefine.IsSharedLandIndex > 0 ? GetString(allItem[currentIndex, familyDefine.IsSharedLandIndex]) : "";
                string record = "";
                if (string.IsNullOrEmpty(person.IsSharedLand))
                {
                    record = string.Format("表中{0}的是否共有人列未填写数据!", person.Name);
                }
                else
                {
                    if (person.IsSharedLand != "是" && person.IsSharedLand != "否")
                    {
                        record = string.Format("表中{0}的是否共有人列数据{1}填写填写错误,内容不是是或否!", person.Name, person.IsSharedLand);
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
            //陈泽林  20161021 如果该共有人是户主，设置默认值
            if (person.Name==family.Name && person.ICN==family.Number)
            {
                person.IsSharedLand = "是";
                person.Relationship = "户主";
            }
            List<Person> personList = family.SharePersonList;
            personList.Add(person);
            family.SharePersonList = personList;
        }

        ///// <summary>
        ///// 设置二轮家庭信息
        ///// </summary>
        ///// <param name="family">户对象</param>
        ///// <param name="familyName">户主名</param>
        ///// <param name="allItem">所有值</param>
        //private void SetTablePersonInfo(VirtualPerson family, string personName, object[,] allItem)
        //{
        //    Person person = new Person();
        //    person.Name = personName;//人名
        //    person.ExtensionPackageNumber = (familyDefine.ExPackageNumberIndex > 0 && familyDefine.ExPackageNumberIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.ExPackageNumberIndex]) : "";//延包土地份数
        //    person.IsDeaded = (familyDefine.IsDeadedIndex > 0 && familyDefine.IsDeadedIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.IsDeadedIndex]) : "";//已死亡人员
        //    person.IsSharedLand = (familyDefine.IsSharedLandIndex > 0 && familyDefine.IsSharedLandIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.IsSharedLandIndex]) : "";//是否享有承包地
        //    person.LocalMarriedRetreatLand = (familyDefine.LocalMarriedRetreatLandIndex > 0 && familyDefine.LocalMarriedRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.LocalMarriedRetreatLandIndex]) : "";//出嫁后未退承包地人员
        //    person.PeasantsRetreatLand = (familyDefine.PeasantsRetreatLandIndex > 0 && familyDefine.PeasantsRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.PeasantsRetreatLandIndex]) : "";//农转非后未退承包地人员
        //    person.ForeignMarriedRetreatLand = (familyDefine.ForeignMarriedRetreatLandIndex > 0 && familyDefine.ForeignMarriedRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, familyDefine.ForeignMarriedRetreatLandIndex]) : "";// 

        //    List<Person> list = new List<Person>();
        //    list.Add(person);
        //    family.SharePersonList = list;
        //    tableFamilys.Add(family);
        //}

        /// <summary>
        /// 获取身份证号码
        /// </summary>
        private string SetPersonIdNumber(string personName, string identifyNumber, eCredentialsType cardType)
        {
            string idNumber = ToolString.ExceptSpaceString(identifyNumber);//身份证号码
            //读数据就读数据，检查数据交给检查数据去做，修改人江宇2016.8.30
            #region
            //if (!OtherDefine.IsPromiseCardNumberNull)
            //{
            //if (FamilyImportDefine.GetIntence().NumberIcnIndex == -1)
            //    return idNumber.ToUpper();
            //if (string.IsNullOrEmpty(idNumber))
            //{
            //    string errorInfo = string.Format("表中{0}的证件号码为空!", personName);
            //    RecordPromptInformation(errorInfo); //记录错误信息
            //    return string.Empty;
            //}
            //if (cardType == eCredentialsType.IdentifyCard && !ToolICN.Check(idNumber))
            //{
            //    string errorInfo = string.Format("表中{0}的身份证号码{1}无效,不符合身份证号码验证规则!", personName, idNumber);
            //    RecordPromptInformation(errorInfo); //记录错误信息
            //    if (!string.IsNullOrEmpty(idNumber) && idNumber.Length != 15 && idNumber.Length != 18)
            //    {
            //        RecordErrorInformation(string.Format("表中{0}的身份证号码{1}共{2}位,不满足身份证号码15位或18位数字要求!",
            //            personName,
            //            idNumber, idNumber.Length));
            //    }
            //    if (!string.IsNullOrEmpty(idNumber) && (idNumber.Length == 15 || idNumber.Length == 18) &&
            //        !ToolMath.MatchEntiretyNumber(idNumber.Replace("x", "").Replace("X", "")))
            //    {
            //        RecordErrorInformation(string.Format("表中{0}的身份证号码{1}共{2}位,但不满足身份证号码数字要求!", personName,
            //            idNumber, idNumber.Length));
            //    }
            //}
            //}
            #endregion
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
        /// 记录提示信息
        /// </summary>
        /// <param name="information"></param>
        private void RecordPromptInformation(string information)
        {
            if (string.IsNullOrEmpty(PromptInformation) || PromptInformation.IndexOf(information) < 0)
            {
                PromptInformation += information;
            }
        }

        #endregion

        #endregion
    }
}
