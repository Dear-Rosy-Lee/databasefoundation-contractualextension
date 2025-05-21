/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 读取二轮台账调查表操作
    /// </summary>
    public class ReadLandInformation : ExcelBase
    {
        #region Field

        private LandImportDefine landDefine;//导入定义
        private SortedList<string, string> existPersons;
        private int currentIndex = 0;//当前索引号
        private string lastRowText = "合计";//最后一行第一个单元格中文字
        private int currentNumber = 0;//当前编号
        private List<LandFamily> landFamilys = new List<LandFamily>();

        /// <summary>
        /// 文件路径
        /// </summary>
        private string fileName = string.Empty;

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext dbContext;

        /// <summary>
        /// 当前行政地域
        /// </summary>
        private Zone currentZone;

        /// <summary>
        /// 错误信息
        /// </summary>
        private string errorInformation = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// 承包户集合
        /// </summary>
        public List<LandFamily> LandFamilyCollection
        {
            get
            {
                return landFamilys;
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext
        {
            get { return dbContext; }
            set { dbContext = value; }
        }

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInformation
        {
            get { return errorInformation; }
            set { errorInformation = value; }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string PromptInformation { get; set; }

        /// <summary>
        /// 是否包含台账承包方数据
        /// </summary>
        public bool IsContaionTableValue
        {
            get { return landDefine.IsContainTableValue; }
        }

        /// <summary>
        /// 是否包含台账地块数据
        /// </summary>
        public bool IsContaionTableLandValue
        {
            get { return landDefine.IsContainTableLandValue; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReadLandInformation(string fileName)
        {
            this.fileName = fileName;
            InitalizeInnerData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeInnerData()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config\" + "LandImportDefine.xml";
            if (File.Exists(filePath))
            {
                landDefine = ToolSerialization.DeserializeXml(filePath, typeof(LandImportDefine)) as LandImportDefine;
            }
            if (landDefine == null)
            {
                landDefine = new LandImportDefine();
                //landDefine.InitalizeLedgerTableValue();
            }
        }

        #endregion

        #region Method—Override

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

        #endregion

        #region Method—获取开始行数据

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

        #endregion

        #region Method—打开表格和读取数据

        /// <summary>
        /// 打开文件
        /// </summary>
        public bool OpenLandFile()
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

        /// <summary>
        /// 读取表格信息
        /// </summary>
        public bool ReadTableInformation()
        {
            object[,] allItem = GetAllRangeValue();//获取所有使用域值
            if (allItem == null)
            {
                RecordErrorInformation("表中可能没有数据或数据可能已经损坏,如果表中有数据请重新建张新表,然后将原数据拷贝过去,再执行该操作!");
                return false;
            }
            LandFamily landFamily = new LandFamily();
            string totalInfo = string.Empty;
            int rowCount = GetRangeRowCount();
            int calIndex = GetStartRow(rowCount, allItem);//获取数据开始行数
            existPersons = new SortedList<string, string>();
            for (int index = calIndex; index < rowCount; index++)
            {
                currentIndex = index;//当前行数
                string rowValue = GetString(allItem[index, 0]);//编号栏数据
                if (rowValue.Trim() == lastRowText || rowValue.Trim() == "总计" || rowValue.Trim() == "共计")
                {
                    break;
                }
                string familyName = landDefine.NameIndex > 0 ? GetString(allItem[currentIndex, landDefine.NameIndex]) : "";
                if (!string.IsNullOrEmpty(rowValue) || !string.IsNullOrEmpty(familyName))
                {
                    int.TryParse(rowValue, out currentNumber);//当前编号
                    landFamily = new LandFamily();
                    if (!string.IsNullOrEmpty(rowValue))
                    {
                        landFamily.CurrentFamily.FamilyNumber = rowValue;
                        if (!ToolMath.MatchEntiretyNumber(rowValue))
                        {
                            string errorInformation = string.Format("表中第{0}行承包方编号{1}不符合数字类型要求!", currentIndex + 1, rowValue);
                            RecordErrorInformation(errorInformation);
                            return false;
                        }
                    }
                    if (!string.IsNullOrEmpty(familyName))
                    {
                        landFamily.CurrentFamily.Name = familyName;
                    }
                    if (string.IsNullOrEmpty(landFamily.CurrentFamily.TotalArea))
                    {
                        landFamily.CurrentFamily.TotalArea = landDefine.SecondTotalTableAreaIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondTotalTableAreaIndex]) : "";
                    }
                    if (string.IsNullOrEmpty(landFamily.CurrentFamily.TotalTableArea.ToString()))
                    {
                        double tableArea = 0;
                        string str = landDefine.SecondTotalTableAreaIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondTotalTableAreaIndex]) : "";
                        double.TryParse(str, out tableArea);
                        landFamily.CurrentFamily.TotalTableArea = tableArea;
                    }
                    existPersons = new SortedList<string, string>();
                    landFamilys.Add(landFamily);
                }
                InitalizeTableFamilyInformation(landFamily, allItem); //获取表格中承包方信息
                InitzlizeTableLandInformation(landFamily, allItem);  //获取表格中地块信息
            }
            allItem = null;
            rowCount = 0;
            currentIndex = 0;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 获取Excel表中承包方信息
        /// </summary>
        private void InitalizeTableFamilyInformation(LandFamily landFamily, object[,] allItem)
        {
            string familyName = landDefine.SecondNameIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNameIndex]) : landFamily.CurrentFamily.Name;
            string value = string.Empty;
            int count = 0;
            if (!string.IsNullOrEmpty(familyName))
            {
                landFamily.TableFamily.Name = familyName;
                if (string.IsNullOrEmpty(landFamily.TableFamily.FamilyNumber))
                {
                    landFamily.TableFamily.FamilyNumber = ToolString.ExceptSpaceString(GetString(allItem[currentIndex, 0]));
                }
                InitializeInnerTableFamily(landFamily);
                value = landDefine.SecondNumberIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberIndex]) : landFamily.CurrentFamily.PersonCount;
                if (!string.IsNullOrEmpty(value))
                {
                    int.TryParse(value, out count);
                }
                if (count > 0)
                {
                    landFamily.TablePersonCount = count;
                    landFamily.TableFamily.PersonCount = count.ToString();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(landFamily.TableFamily.Name) && landDefine.SecondNameIndex > 0)
                {
                    string errorInfo = string.Format("序号为{0}的承包方姓名为空!", currentIndex);
                    RecordErrorInformation(errorInfo);  //记录错误信息
                }
            }
            InitalizeTablePerson(landFamily, allItem);
        }

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        private void InitializeInnerTableFamily(LandFamily landFamily)
        {
            landFamily.TableFamily.ZoneCode = currentZone.FullCode;
            landFamily.TableFamily.ModifiedTime = DateTime.Now;
            landFamily.TableFamily.CreationTime = DateTime.Now;
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private bool InitalizeTablePerson(LandFamily landFamily, object[,] allItem)
        {
            string value = string.Empty;
            Person person = new Person();
            person.FamilyID = landFamily.TableFamily.ID;
            person.CreateTime = DateTime.Now;
            person.LastModifyTime = DateTime.Now;
            person.Nation = eNation.UnKnown;
            person.ZoneCode = currentZone.FullCode;
            //性别
            value = landDefine.SecondNumberGenderIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberGenderIndex]) : "";
            person.Gender = string.IsNullOrEmpty(value) ? eGender.Unknow : GetGender(value);
            //2011年1月18日 0:12:50 Roc 增加从 Excel 中获取填写的年龄
            //年龄
            int age = 0;
            string strAge = landDefine.SecondNumberAgeIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberAgeIndex]) : "";
            Int32.TryParse(strAge, out age);
            //身份证号
            string icn = landDefine.SecondNumberIcnIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberIcnIndex]) : "";
            //家庭关系
            person.Relationship = landDefine.SecondNumberRelatioinIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberRelatioinIndex]) : "";
            //备注
            person.Comment = landDefine.SecondFamilyCommentIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondFamilyCommentIndex]) : "";
            //名称
            string name = landDefine.SecondNumberNameIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNumberNameIndex]) : "";
            if (string.IsNullOrEmpty(name))
            {
                name = landDefine.ExPackageNameIndex > 0 ? GetString(allItem[currentIndex, landDefine.ExPackageNameIndex]) : "";//家庭成员名
            }
            if (string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(value) || age != 0 || !string.IsNullOrEmpty(icn)) && (landDefine.SecondNumberNameIndex > 0 || landDefine.ExPackageNameIndex > 0))
            {
                RecordErrorInformation(string.Format("序号为{0}的家庭成员姓名为空!", currentIndex));
            }
            person.Name = name;
            InitalizeTablePersonInformation(landFamily, person, age, icn);
            InitalizeTablePersonExtendInformation(person, allItem);
            if (string.IsNullOrEmpty(landFamily.TableFamily.TotalArea))
            {
                landFamily.TableFamily.TotalArea = landDefine.SecondTotalTableAreaIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondTotalTableAreaIndex]) : "";//台账总面积
            }
            if (landFamily.TableFamily.TotalTableArea == 0)
            {
                double tableArea = 0;
                string str = landDefine.SecondTotalTableAreaIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondTotalTableAreaIndex]) : "";
                double.TryParse(str, out tableArea);
                landFamily.TableFamily.TotalTableArea = tableArea;
            }
            if (!string.IsNullOrEmpty(person.Name))
            {
                landFamily.TablePersons.Add(person);
            }
            if (person.Name == landFamily.TableFamily.Name)
            {
                landFamily.TableFamily.SourceID = person.ID;
                landFamily.TableFamily.Number = person.ICN;
            }
            return true;
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
        /// 初始化家庭成员信息
        /// </summary>
        private void InitalizeTablePersonInformation(LandFamily landFamily, Person person, int age, string icn)
        {
            if (age > 0)
            {
                person.Birthday = DateTime.Now.Date.AddYears(-age);
            }
            if (string.IsNullOrEmpty(icn))
            {
                person.ICN = "";
                if (!string.IsNullOrEmpty(person.Name) && (landDefine.SecondNumberNameIndex > 0 || landDefine.ExPackageNameIndex > 0))
                {
                    RecordPromptInformation("共有人" + person.Name + "的身份证号码为空");
                }
            }
            else
            {
                person.ICN = icn;
                VerifyTablePersonNumber(person);
                if (!string.IsNullOrEmpty(icn) && icn.Length != 15 && icn.Length != 18 && landDefine.SecondNumberIcnIndex > 0)
                {
                    RecordErrorInformation(string.Format("表中共有人{0}的身份证号码{1}共{2}位,不满足身份证号码15位或18位要求!", person.Name, icn, icn.Length));
                }
            }
            if (person.Gender == eGender.Unknow && !string.IsNullOrEmpty(person.Name) && landDefine.SecondNumberGenderIndex > 0)
            {
                RecordPromptInformation("共有人" + person.Name + "的性别填写不正确!");
            }
            //检查同户中存在同名成员的情况
            if (!string.IsNullOrEmpty(person.Name) && (landDefine.SecondNumberNameIndex > 0 || landDefine.ExPackageNameIndex > 0))
            {
                if (existPersons.ContainsKey(person.Name))
                {
                    RecordPromptInformation("承包方" + landFamily.TableFamily.Name + "下存在同名的成员：" + person.Name);
                }
                else
                {
                    existPersons.Add(person.Name, person.Name);
                }
            }
        }

        /// <summary>
        /// 设置共有人值
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private void VerifyTablePersonNumber(Person person)
        {
            if (string.IsNullOrEmpty(person.ICN))
            {
                return;
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
                    if (ToolICN.Check(person.ICN) && landDefine.SecondNumberIcnIndex > 0)
                    {
                        RecordPromptInformation("共有人" + person.Name + "的身份证号码：" + person.ICN + "不符合身份证验证规则!");
                    }
                }
            }
        }

        /// <summary>
        /// 设置家庭信息
        /// </summary>
        /// <param name="family">户对象</param>
        /// <param name="familyName">户主名</param>
        /// <param name="allItem">所有值</param>
        private void InitalizeTablePersonExtendInformation(Person person, object[,] allItem)
        {
            person.ExtensionPackageNumber = landDefine.ExPackageNumberIndex > 0 ? GetString(allItem[currentIndex, landDefine.ExPackageNumberIndex]) : "";//延包土地份数
            person.IsDeaded = landDefine.IsDeadedIndex > 0 ? GetString(allItem[currentIndex, landDefine.IsDeadedIndex]) : "";//已死亡人员
            person.IsSharedLand = landDefine.IsSharedLandIndex > 0 ? GetString(allItem[currentIndex, landDefine.IsSharedLandIndex]) : "";//是否享有承包地
            person.LocalMarriedRetreatLand = landDefine.LocalMarriedRetreatLandIndex > 0 ? GetString(allItem[currentIndex, landDefine.LocalMarriedRetreatLandIndex]) : "";//出嫁后未退承包地人员
            person.PeasantsRetreatLand = landDefine.PeasantsRetreatLandIndex > 0 ? GetString(allItem[currentIndex, landDefine.PeasantsRetreatLandIndex]) : "";//农转非后未退承包地人员
            person.ForeignMarriedRetreatLand = landDefine.ForeignMarriedRetreatLandIndex > 0 ? GetString(allItem[currentIndex, landDefine.ForeignMarriedRetreatLandIndex]) : "";//婚进在婚出地未退承包地
        }

        /// <summary>
        /// 获取Excel表中地块信息
        /// </summary>
        private void InitzlizeTableLandInformation(LandFamily landFamily, object[,] allItem)
        {
            SecondTableLand land = new SecondTableLand();
            //地籍编码
            land.CadastralNumber = currentZone.FullCode + "000" + string.Format("{0:D4}", currentIndex);
            InitializeInnerTableLand(land, landFamily);
            //小地名
            land.Name = landDefine.SecondLandNameIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondLandNameIndex]) : "";
            //承包地台账面积
            land.TableArea = landDefine.SecondTableAreaIndex > 0 ? GetDouble(allItem[currentIndex, landDefine.SecondTableAreaIndex]) : 0.0;
            //地类
            land.LandName = landDefine.SecondLandTypeIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondLandTypeIndex]) : "";
            if (!string.IsNullOrEmpty(land.LandName))
            {
                //ModuleMsgArgs arg = MessageExtend.SecondTableMsg(dbContext, SecondTableLandMessage.SECONDLAND_GET_DICTIONARY, "", "");
                //TheBns.Current.Message.Send(this, arg);
                //List<Dictionary> listDict = arg.ReturnValue as List<Dictionary>;
                //land.LandCode = listDict.Find(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == "C26" && (c.Name.Equals(land.LandName) || c.Name.Contains(land.LandName))).Code;
            }
            else
            {
                land.LandCode = "XX";
            }
            //四至信息
            land.NeighborEast = landDefine.SecondEastIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondEastIndex]) : "";
            land.NeighborNorth = landDefine.SecondNorthIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondNorthIndex]) : "";
            land.NeighborSouth = landDefine.SecondSourthIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondSourthIndex]) : "";
            land.NeighborWest = landDefine.SecondWestIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondWestIndex]) : "";
            //备注
            land.Comment = landDefine.SecondCommentIndex > 0 ? GetString(allItem[currentIndex, landDefine.SecondCommentIndex]) : "";
            if (string.IsNullOrEmpty(land.LandName) && string.IsNullOrEmpty(land.Name) && string.IsNullOrEmpty(land.Comment)
                && (land.TableArea == null || !land.TableArea.HasValue || land.TableArea.Value == 0.0))
            {
                return;
            }
            if (land.TableArea < 0.0 && landDefine.SecondTableAreaIndex > 0)
            {
                RecordErrorInformation(land.OwnerName + "下地块承包地台账面积填写错误!");
            }
            land.ActualArea = land.AwareArea = land.TableArea.Value;
            landFamily.TableLandCollection.Add(land);
        }

        /// <summary>
        /// 初始化内部承包地块
        /// </summary>
        /// <param name="land"></param>
        /// <param name="landFamily"></param>
        private void InitializeInnerTableLand(SecondTableLand land, LandFamily landFamily)
        {
            land.CadastralNumber = string.IsNullOrEmpty(land.CadastralNumber) ? currentIndex.ToString() : land.CadastralNumber;
            land.LandNumber = land.CadastralNumber;
            land.OwnerName = landFamily.TableFamily.Name;
            land.OwnerId = landFamily.TableFamily.ID;
            land.SenderCode = currentZone.FullCode;
            land.SenderName = currentZone.FullName;
            land.ZoneCode = currentZone.FullCode;
            land.ZoneName = currentZone.FullName;
            land.AwareArea = land.ActualArea;//发证面积为了打证而用。
            land.IsFlyLand = false;
            land.IsTransfer = false;
            ModuleMsgArgs arg = MessageExtend.SecondTableMsg(dbContext, SecondTableLandMessage.SECONDLAND_GET_DICTIONARY, "", "");
            TheBns.Current.Message.Send(this, arg);
            List<Dictionary> listDict = arg.ReturnValue as List<Dictionary>;
            GetDictValue(land, listDict);
        }

        /// <summary>
        /// 从数据字典中获取地块相应信息
        /// </summary>
        /// <param name="land">二轮地块</param>
        /// <param name="listDict">字典集合</param>
        private void GetDictValue(SecondTableLand land, List<Dictionary> listDict)
        {
            land.Purpose = listDict.Find(c => c.GroupCode == "C9" && c.Code == "5").Name;
            land.LandLevel = listDict.Find(c => c.GroupCode == "C8" && c.Code == "900").Name;
            land.TransferType = listDict.Find(c => c.GroupCode == "C23" && c.Code == "6").Name;
            land.LandCategory = listDict.Find(c => c.GroupCode == "C7" && c.Code == "10").Name;
            land.LandScopeLevel = listDict.Find(c => c.GroupCode == "C21" && c.Code == "9").Name;
            land.OwnRightType = listDict.Find(c => c.GroupCode == "C6" && c.Code == "30").Name;
        }

        #endregion

        #region Method—Error

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

    }
}
