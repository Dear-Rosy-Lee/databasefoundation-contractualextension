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
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化承包台账信息
    /// </summary>
    [Serializable]
    public partial class InitalizeLandSurveyInformation : ExcelBase
    {
        #region Fields

        private const double PRECISION = 0.000001;//精度
        private Zone currentZone;
        private IDbContext dbContext;
        private object[,] allItem;
        private int rangeCount;//行数
        private int columnCount;//列数
        private bool isOk;
        private bool allowNoWriteActualArea;//允许不填写实测面积
        private bool allowNoWriteAwareArea;//允许不填写确权面积
        private bool allowAwareAreaBigActualArea;//允许确权面积大于实测面积
        private bool contractorClear;//是否清空承包方

        private SortedList<string, string> existPersons;
        private SortedList<string, string> existTablePersons;

        private string lastRowText = "合计";//最后一行第一个单元格中文字
        private string fileName = string.Empty;//文件名称
        private int currentIndex = 0;//当前索引号
        private int currentNumber = 0;//当前编号

        //3.14需求
        private string concordNumber = string.Empty;//合同编号
        private string bookNumber = string.Empty;//证书编号

        private double countActualArea = 0.0;//当前行实测总面积
        private double countAwareArea = 0.0;//当前行确权总面积
        private double countMotorizeLandArea = 0.0;//当前行机动地总面积
        private double countTotalTableArea = 0.0;//二轮承包地总面积

        private List<string> errorArray=new List<string> ();//错误信息
        private List<string> warnArray=new List<string>();//警告信息

        #endregion

        #region Propertys

        /// <summary>
        /// 导入excel表的名称
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        private ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 承包台账导入设置实体
        /// </summary>
        private ContractBusinessImportSurveyDefine ContractLandImportSurveyDefine =
            ContractBusinessImportSurveyDefine.GetIntence();

        /// <summary>
        /// 承包户集合
        /// </summary>
        public List<LandFamily> LandFamilyCollection
        {
            get { return _landFamilyCollection; }
            set { _landFamilyCollection = value; }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName
        {
            set { fileName = value; }
        }

        /// <summary>
        /// 当前行政区域
        /// </summary>
        public Zone CurrentZone
        {
            set { currentZone = value; }
        }

        /// <summary>
        /// 是否包含二轮台账承包方数据
        /// </summary>
        public bool IsContaionTableValue
        {
            get { return ContractLandImportSurveyDefine.IsContainTableValue; }
        }

        /// <summary>
        /// 是否包含二轮台账地块数据
        /// </summary>
        public bool IsContaionTableLandValue
        {
            get { return ContractLandImportSurveyDefine.IsContainTableLandValue; }
        }

        /// <summary>
        /// 是否含有承包方扩展信息
        /// </summary>
        public bool IsContainFamilyExpandValue { get; set; }

        /// <summary>
        /// 是否包含二轮承包方扩展信息
        /// </summary>
        public bool isContainTableFamilyExpandValue { get; set; }

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DbContext
        {
            set { dbContext = value; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public List<string> ErrorInformation
        {
            get { return errorArray; }
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        public List<string> WarnInformation
        {
            get { return warnArray; }
        }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 字典
        /// </summary>
        public List<Dictionary> DictList
        { get; set; }

        /// <summary>
        /// 地块类别
        /// </summary>
        public List<Dictionary> listDKLB
        { get; set; }

        /// <summary>
        /// 地块等级
        /// </summary>
        public List<Dictionary> listDLDJ
        { get; set; }

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public List<Dictionary> listTDLYLX
        { get; set; }

        /// <summary>
        /// 耕地坡度级别
        /// </summary>
        public List<Dictionary> listGDPDJ
        { get; set; }

        /// <summary>
        /// 流转类型
        /// </summary>
        public List<Dictionary> listLZLX
        { get; set; }

        /// <summary>
        /// 耕保种类
        /// </summary>
        public List<Dictionary> listGBZL
        { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public List<Dictionary> listCBJYQQDFS
        { get; set; }

        /// <summary>
        /// 所有权性质
        /// </summary>
        public List<Dictionary> listSYQXZ
        { get; set; }

        /// <summary>
        /// 土地用途
        /// </summary>
        public List<Dictionary> listTDYT
        { get; set; }

        /// <summary>
        /// 土地用途
        /// </summary>
        public List<Dictionary> listZZLX
        { get; set; }

        private ContractBusinessSettingDefine _config = ContractBusinessSettingDefine.GetIntence();

        private List<LandFamily> _landFamilyCollection = new List<LandFamily>();

        #endregion

        #region Ctor

        public InitalizeLandSurveyInformation(List<Dictionary> listDict)
        {
            if (listDict == null)
            {
                return;
            }
            LandFamilyCollection = new List<LandFamily>();
            DictList = listDict;
            listDKLB = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            listDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            listTDLYLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX);
            listGDPDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.GDPDJ);
            listLZLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.LZLX);
            listGBZL = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.GBZL);
            listCBJYQQDFS = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            listSYQXZ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.SYQXZ);
            listTDYT = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
            listZZLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.ZZLX);
        }

        /// <summary>
        /// 初始化数据, 设置ContractLandImportSurveyDefine
        /// </summary>
        public void InitalizeInnerData()
        {
            IsContainFamilyExpandValue = ContractLandImportSurveyDefine.InitalzieCencusTableValue();
            isContainTableFamilyExpandValue = ContractLandImportSurveyDefine.InitalizeTableFamilyValue();
            allowNoWriteActualArea = SettingDefine.WriteActualArea;//允许不填写实测面积
            allowNoWriteAwareArea = SettingDefine.WriteRightArea;//允许不填写确权面积
            allowAwareAreaBigActualArea = SettingDefine.RightAreaMoreActualArea;//允许确权面积大于实测面积
            contractorClear = SettingDefine.ClearVirtualPersonData;
        }

        #endregion

        #region Methods

        #region Methods - Override

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Disponse()
        {
            allItem = null;
            LandFamilyCollection.Clear();
            LandFamilyCollection = null;
            errorArray = null;
            warnArray = null;
            existTablePersons = null;
            existPersons = null;
            currentZone = null;
            DbContext = null;
            rangeCount = 0;
            columnCount = 0;
            currentIndex = 0;
            currentNumber = 0;
        }

        #endregion

        #region Methods - Check


        /// <summary>
        /// 检查合同编号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CheckConcordNumber(int index)
        {
            SortedList<string, object> cnumber = new SortedList<string, object>();
            SortedList<string, object> rnumber = new SortedList<string, object>();

            for (; index < rangeCount; index++)
            {
                string temp = GetString(allItem[index, 0]);
                if (temp == "合计")
                    break;
                if (!string.IsNullOrEmpty(temp))
                {
                    string tempi = (ContractLandImportSurveyDefine.ConcordIndex > 0 && ContractLandImportSurveyDefine.ConcordIndex < columnCount) ? GetString(allItem[index, ContractLandImportSurveyDefine.ConcordIndex]) : "";//合同编号
                    string tempj = (ContractLandImportSurveyDefine.RegeditBookIndex > 0 && ContractLandImportSurveyDefine.RegeditBookIndex < columnCount) ? GetString(allItem[index, ContractLandImportSurveyDefine.RegeditBookIndex]) : "";//证书编号

                    if (cnumber.ContainsKey(tempi) || cnumber.Keys.IndexOf(tempi) >= 0)
                        AddErrorMessage("已经存在的合同编号：" + tempi);

                    if (rnumber.ContainsKey(tempj) || rnumber.Keys.IndexOf(tempj) >= 0)
                        AddErrorMessage("已经存在的证书编号：" + tempj);

                    if (!string.IsNullOrEmpty(tempi) && !string.IsNullOrEmpty(tempj))
                    {
                        //if (tempi.Length < 17 || tempi.Length > 20)//合同编号不一定是16位地域编码+4位流水号，需兼容以前已经存在的合同号
                        //    AddErrorMessage("位数错误的合同编号：" + tempi + "，位数应为20位(16位地域编码+4位流水号)");

                        if (!cnumber.ContainsKey(tempi))
                            cnumber.Add(tempi, null);
                        if (!rnumber.ContainsKey(tempj))
                            rnumber.Add(tempj, null);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 检查值
        /// </summary>
        private bool CheckValue()
        {
            InitializeFields();

            if (!OpenExcel())
                return false;
            if (!SetValue())
                return false;
            return true;
        }

        /// <summary>
        /// 检查地块编号
        /// </summary>
        private bool CheckCadastralNumber(string cadastralNumber, bool isCheckLandNumberRepeat)
        {
            if (isCheckLandNumberRepeat == false) return true;
            foreach (LandFamily item in LandFamilyCollection)
            {
                foreach (var land in item.LandCollection)
                {
                    if (land.CadastralNumber == cadastralNumber || land.CadastralNumber.Equals(cadastralNumber))
                    {
                        AddErrorMessage(this.ExcelName + string.Format("表序号{0}中的地块编码" +
                                                                       "{1}已存在!", currentIndex + 1, ContractLand.GetLandNumber(cadastralNumber)));
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检查地块编号
        /// </summary>
        private bool CheckCadastralNumber(LandFamily land, string cadastralNumber, bool isCheckLandNumberRepeat)
        {
            if (isCheckLandNumberRepeat == false) return true;
            foreach (var item in land.LandCollection)
            {
                if (item.CadastralNumber == cadastralNumber || item.CadastralNumber.Equals(cadastralNumber))
                {
                    string info = this.ExcelName + string.Format("表序号{0}中的地块编码{1}已存在!", currentIndex + 1, ContractLand.GetLandNumber(cadastralNumber));
                    AddErrorMessage(info);
                }
            }
            return true;
        }

        /// <summary>
        /// 检查合同面积
        /// </summary>
        private bool CheckConcordDataValue(LandFamily landFamily)
        {
            if (landFamily.Concord.CountActualArea - landFamily.Concord.CountAwareArea < 0.0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下确权总面积大于实测总面积！");
            }
            if (landFamily.Concord.TotalTableArea < 0.0 && ContractLandImportSurveyDefine.TotalTableAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下总二轮承包地面积小于0！");
            }
            if (landFamily.Concord.CountMotorizeLandArea < 0.0 && ContractLandImportSurveyDefine.TotalMotorizeAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下总机动地面积小于0！");
            }

            if (landFamily.Concord.CountActualArea < 0.0 && ContractLandImportSurveyDefine.TotalActualAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下总实测面积面积小于0！");
            }

            if (landFamily.Concord.CountAwareArea < 0.0 && ContractLandImportSurveyDefine.TotalAwareAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下总确权面积小于0！");
            }
            if (ContractLandImportSurveyDefine.TotalMotorizeAreaIndex > 0 && Math.Abs(landFamily.Concord.CountMotorizeLandArea - countMotorizeLandArea) > PRECISION)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下机动地总面积与其地块机动地面积之和不等！");
            }
            if (Math.Abs(landFamily.Concord.CountActualArea - countActualArea) > PRECISION && ContractLandImportSurveyDefine.TotalActualAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下实测总面积与其地块实测面积之和不等！");
            }
            if (Math.Abs(landFamily.Concord.CountAwareArea - countAwareArea) > PRECISION && ContractLandImportSurveyDefine.TotalAwareAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下确权总面积与其地块确权面积之和不等！");
            }
            if (Math.Abs(Math.Abs((landFamily.Concord.CountActualArea - landFamily.Concord.CountAwareArea)) - landFamily.Concord.CountMotorizeLandArea) > PRECISION && ContractLandImportSurveyDefine.TotalMotorizeAreaIndex > 0)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下实测总面积减去确权总面积不等于总机动地面积！");
            }
            return true;
        }

        /// <summary>
        /// 检查导入数据
        /// </summary>
        public bool CheckImportData(List<LandFamily> landFamilyCollection)
        {

            bool check = true;
            for (int i = 0; i < landFamilyCollection.Count; i++)
            {
                LandFamily onefamily = landFamilyCollection[i];
                if (string.IsNullOrEmpty(onefamily.CurrentFamily.Name))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(onefamily.CurrentFamily.FamilyNumber))
                {
                    AddErrorMessage(this.ExcelName + "表中承包方" + onefamily.CurrentFamily.Name + "的编号为空!");
                    continue;
                }
                if (contractorClear)
                {
                    if (onefamily.Persons == null || onefamily.Persons.Count == 0 || onefamily.Persons.Find(fm => fm.Name == onefamily.CurrentFamily.Name) == null)
                    {
                        RecordErrorInformation(this.ExcelName + "表中承包方" + onefamily.CurrentFamily.Name + "未包含在其家庭成员中!");
                        continue;
                    }
                }
                if (onefamily.PersonCount != -1 && onefamily.PersonCount != onefamily.Persons.Count)
                {
                    RecordErrorInformation(this.ExcelName + "表中承包方" + onefamily.CurrentFamily.Name + "家庭成员个数与家庭成员数不一致!");
                }
                for (int j = i + 1; j < landFamilyCollection.Count; j++)
                {
                    LandFamily twofamily = landFamilyCollection[j];
                    if (onefamily.CurrentFamily.Name == twofamily.CurrentFamily.Name && string.IsNullOrEmpty(onefamily.CurrentFamily.Number))
                    {
                        string warnInfo = this.ExcelName + string.Format("表承包方{0}在表中重复存在!", onefamily.CurrentFamily.Name);
                        AddErrorMessage(warnInfo);
                    }
                    if (onefamily.CurrentFamily.Name == twofamily.CurrentFamily.Name && onefamily.CurrentFamily.Number != twofamily.CurrentFamily.Number)
                    {
                        string warnInfo = this.ExcelName + string.Format("表承包方{0}在表中重复存在!", onefamily.CurrentFamily.Name);
                        AddErrorMessage(warnInfo);
                    }
                    if (onefamily.CurrentFamily.Name == twofamily.CurrentFamily.Name && onefamily.CurrentFamily.Number == twofamily.CurrentFamily.Number && !string.IsNullOrEmpty(onefamily.CurrentFamily.Number))
                    {
                        string errorInfo = this.ExcelName + string.Format("表承包方{0}在表中重复存在，并且身份证号码{1}相同!", onefamily.CurrentFamily.Name, onefamily.CurrentFamily.Number);
                        AddErrorMessage(errorInfo);
                        check = false;
                    }
                    if (onefamily.CurrentFamily.FamilyNumber == twofamily.CurrentFamily.FamilyNumber && !string.IsNullOrEmpty(onefamily.CurrentFamily.FamilyNumber))
                    {
                        int number = 0;
                        Int32.TryParse(onefamily.CurrentFamily.FamilyNumber, out number);
                        string errorInfo = this.ExcelName + string.Format("表承包方{0}编号{1}与承包方{2}编号{3}在表中重复存在!", onefamily.CurrentFamily.Name, number, twofamily.CurrentFamily.Name, number);
                        AddErrorMessage(errorInfo);
                        check = false;
                    }
                    for (int l = 0; l < onefamily.Persons.Count; l++)
                    {
                        Person per = onefamily.Persons[l];
                        if (string.IsNullOrEmpty(per.ICN))
                        {
                            continue;
                        }
                        for (int k = l + 1; k < onefamily.Persons.Count; k++)
                        {
                            Person pe = onefamily.Persons[k];
                            if (string.IsNullOrEmpty(pe.ICN))
                            {
                                continue;
                            }

                            if (!_config.CheckVirtualPersonSurveyTable)
                            {
                                if (pe.ICN == per.ICN)
                                {
                                    string errorInfo = this.ExcelName + string.Format("表家庭成员中{0}与{1}的身份证号码({2})重复!", per.Name, pe.Name, per.ICN);
                                    if (SettingDefine.CheckVirtualPersonSurveyTable)//身份证重复
                                    {
                                        AddWarnMessage(errorInfo);
                                    }
                                    else
                                    {
                                        AddErrorMessage(errorInfo);
                                        check = false;
                                    }
                                }
                            }
                        }
                    }
                    foreach (Person p1 in onefamily.Persons)
                    {
                        if (string.IsNullOrEmpty(p1.ICN))
                        {
                            continue;
                        }
                        foreach (Person p2 in twofamily.Persons)
                        {
                            if (string.IsNullOrEmpty(p2.ICN))
                            {
                                continue;
                            }
                            if (p1.ICN == p2.ICN)
                            {
                                string errorInfo = this.ExcelName + string.Format("表家庭成员{0}与家庭成员{1}身份证号码{2}重复!", p1.Name, p2.Name, p2.ICN);
                                if (SettingDefine.CheckVirtualPersonSurveyTable)
                                {
                                    AddWarnMessage(errorInfo);
                                }
                                else
                                {
                                    AddErrorMessage(errorInfo);
                                    check = false;
                                }
                            }
                        }
                    }
                }
            }
            return check;
        }



        #endregion

        #region Methods - Private

        /// <summary>
        /// 读取表格信息
        /// </summary>
        public bool ReadTableInformation(bool isNotLand, bool isCheckLandNumberRepeat)
        {
            bool isAdd = false;
            int concordIndex = 1;
            if (!CheckValue())
            {
                return false;
            }
            LandFamily landFamily = new LandFamily();
            string totalInfo = string.Empty;
            try
            {
                int calIndex = GetStartRow();//获取数据开始行数
                if (calIndex == -1)
                {
                    ReportErrorInfo(this.ExcelName + "表中无数据或数据未按要求制作!");
                    return false;
                }
                existPersons = new SortedList<string, string>();
                existTablePersons = new SortedList<string, string>();
                if (!CheckConcordNumber(calIndex))
                {
                    return false;
                }
                for (int index = calIndex; index < rangeCount; index++)
                {
                    currentIndex = index;//当前行数
                    string rowValue = GetString(allItem[index, 0]);//编号栏数据
                    if (rowValue.Trim() == lastRowText || rowValue.Trim() == "总计" || rowValue.Trim() == "共计")
                    {
                        if (concordIndex != 1)//在添加L装载其他属性值
                        {
                            landFamily = InitalizeConcord(landFamily);
                        }
                        if (landFamily == null)
                        {
                            return false;
                        }
                        if (isAdd && !AddLandFamily(landFamily))
                        {
                            return false;
                        }
                        break;
                    }
                    string familyName = (ContractLandImportSurveyDefine.NameIndex > 0 && ContractLandImportSurveyDefine.NameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NameIndex]) : "";
                    if (!string.IsNullOrEmpty(rowValue) || !string.IsNullOrEmpty(familyName))
                    {
                        if (concordIndex != 1)//装载其他属性值
                        {
                            landFamily = InitalizeConcord(landFamily); //添加扩展属性
                            if (landFamily == null)
                            {
                                continue;
                            }
                        }
                        if (isAdd && !AddLandFamily(landFamily)) //添加户与承包地块
                        {
                            continue;
                        }
                        landFamily = NewFamily(landFamily, rowValue, currentIndex, concordIndex);//重新创建
                        concordIndex++;
                        isAdd = true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(landFamily.CurrentFamily.FamilyNumber))
                        {
                            ReportErrorInfo(this.ExcelName + string.Format("表中第{0}行承包方编号未填写内容!", index));
                            continue;
                        }
                    }
                    GetExcelInformation(landFamily, isNotLand,isCheckLandNumberRepeat);//获取Excel表中信息
                }
            }
            catch (Exception ex)
            {
                Dispose();
                YuLinTu.Library.Log.Log.WriteException(this, "ReadTableInformation(读取表格信息失败)", ex.Message + ex.StackTrace);
                return ReportErrorInfo(this.ExcelName + "转换过程中发生错误：" + ex.Message.ToString() + ",请确认导入数据是否完整匹配!");
            }
            return isOk;
        }

        /// <summary>
        /// 设置编号
        /// </summary>
        private LandFamily SetNumber(LandFamily landFamily, string cnumber, string rnumber)
        {
            landFamily.Concord.ConcordNumber = cnumber;//合同编号
            landFamily.RegeditBook.RegeditNumber = rnumber;//证书编号
            landFamily.RegeditBook.Number = rnumber;//证书编号
            return landFamily;
        }

        /// <summary>
        /// 新承包方
        /// </summary>
        private LandFamily NewFamily(LandFamily landFamily, string rowValue, int currentIndex, int concordIndex)
        {
            int.TryParse(rowValue, out currentNumber);//当前编号
            landFamily = new LandFamily();
            landFamily.Number = currentNumber;
            string ctemp = (ContractLandImportSurveyDefine.ConcordIndex > 0 && ContractLandImportSurveyDefine.ConcordIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ConcordIndex]) : "";//合同编号
            string rtemp = (ContractLandImportSurveyDefine.RegeditBookIndex > 0 && ContractLandImportSurveyDefine.RegeditBookIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.RegeditBookIndex]) : "";//证书编号
            countActualArea = (ContractLandImportSurveyDefine.TotalActualAreaIndex > 0 && ContractLandImportSurveyDefine.TotalActualAreaIndex < columnCount) ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TotalActualAreaIndex]) : 0.0;//当前行实测总面积
            countAwareArea = (ContractLandImportSurveyDefine.TotalAwareAreaIndex > 0 && ContractLandImportSurveyDefine.TotalAwareAreaIndex < columnCount) ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TotalAwareAreaIndex]) : 0.0;//当前行确权总面积
            countMotorizeLandArea = (ContractLandImportSurveyDefine.TotalMotorizeAreaIndex > 0 && ContractLandImportSurveyDefine.TotalMotorizeAreaIndex < columnCount) ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TotalMotorizeAreaIndex]) : 0.0;//当前行机动地总面积
            countTotalTableArea = (ContractLandImportSurveyDefine.TotalTableAreaIndex > 0 && ContractLandImportSurveyDefine.TotalTableAreaIndex < columnCount) ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TotalTableAreaIndex]) : 0.0;//当前行二轮承包地总面积
            if ((!string.IsNullOrEmpty(ctemp) && !string.IsNullOrEmpty(rtemp)))
            {
                landFamily = SetNumber(landFamily, ctemp, rtemp);
            }
            return landFamily;
        }

        /// <summary>
        /// 初始化字段信息
        /// </summary>
        private void InitializeFields()
        {
            isOk = true;
            concordNumber = string.Empty;//合同编号
            bookNumber = string.Empty;//证书编号
        }

        /// <summary>
        /// 添加户与承包地块
        /// </summary>
        private bool AddLandFamily(LandFamily landFamily)
        {
            if (contractorClear)
            {
                if (landFamily.CurrentFamily.SourceID == null || landFamily.CurrentFamily.SourceID.Value == Guid.Empty)
                {
                    AddErrorMessage(this.ExcelName + string.Format("表中名称为{0}的承包方下家庭成员中不包含户主信息!", landFamily.CurrentFamily.Name));
                }
            }
            existPersons = new SortedList<string, string>();
            existTablePersons = new SortedList<string, string>();
            LandFamilyCollection.Add(landFamily);
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        private bool SetValue()
        {
            allItem = GetAllRangeValue();
            rangeCount = GetRangeRowCount();
            columnCount = GetRangeColumnCount();
            if (allItem == null || allItem.Length < 1)
                return ReportErrorInfo(this.ExcelName + "表中可能没有数据或数据可能已经损坏,如果表中有数据请重新建张新表,然后将原数据拷贝过去,再执行该操作!");
            if (rangeCount < 1 || columnCount < 1)
                return ReportErrorInfo(this.ExcelName + "表中可能没有数据或数据可能已经损坏,如果表中有数据请重新建张新表,然后将原数据拷贝过去,再执行该操作!");
            return true;
        }

        /// <summary>
        /// 打开表格
        /// </summary>
        private bool OpenExcel()
        {
            try
            {
                Open(fileName);
            }
            catch (Exception ex)
            {
                Dispose();
                YuLinTu.Library.Log.Log.WriteException(this, "OpenExcel(打开表格失败)", ex.Message + ex.StackTrace);
                return ReportErrorInfo("打开Excel文件时出错,错误信息：" + ex.Message.ToString());
            }
            return true;
        }

        /// <summary>
        /// 获取开始行数据
        /// </summary>
        private int GetStartRow()
        {
            int startIndex = -1;
            for (int i = 0; i < rangeCount; i++)
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
            return startIndex == 0 ? -1 : startIndex;
        }

        /// <summary>
        /// 获取Excel中信息
        /// </summary>
        private void GetExcelInformation(LandFamily landFamily, bool isNotLand, bool isCheckLandNumberRepeat)
        {
            try
            {
                InitalizeFamilyInformation(landFamily);
                if (ContractLandImportSurveyDefine.IsContainTableValue)
                {
                    InitalizeTableFamilyInformation(landFamily);
                }
                InitalizeLandInformation(landFamily, isNotLand, isCheckLandNumberRepeat);
                if (ContractLandImportSurveyDefine.IsContainTableLandValue)
                {
                    InitzlizeTableLandInformation(landFamily);
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetExcelInformation(获取Excel表中信息失败)", ex.Message + ex.StackTrace);
                ReportErrorInfo("读取表格信息失败，" + string.Format("请检查导入地块调查表配置是否与{0}表结构匹配", this.ExcelName));
            }
        }

        #endregion

        #region Methods - Error

        /// <summary>
        /// 记录错误信息
        /// </summary>      
        private void RecordErrorInformation(string errorInfo)
        {
            isOk = false;
            AddErrorMessage(errorInfo);
        }

        /// <summary>
        /// 报告错误信息
        /// </summary>
        private bool ReportErrorInfo(string message)
        {
            isOk = false;
            AddErrorMessage(message);
            return false;
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        private void AddErrorMessage(string message)
        {
            if (errorArray == null)
                errorArray = new List<string>();
            if (!errorArray.Contains(message))
                errorArray.Add(message);
        }

        /// <summary>
        /// 添加警告信息
        /// </summary>
        private void AddWarnMessage(string message)
        {
            if (warnArray == null)
                warnArray = new List<string>();
            if (!warnArray.Contains(message))
                warnArray.Add(message);
        }

        #endregion

        #endregion
    }
}

