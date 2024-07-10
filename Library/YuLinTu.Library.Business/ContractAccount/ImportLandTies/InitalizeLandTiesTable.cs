using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    [Serializable]
    public class InitalizeLandTiesTable : ExcelBase
    {
        #region Fields

        private const double PRECISION = 0.000001;//精度
        private Zone currentZone;
        private IDbContext dbContext;
        private object[,] allItem;
        private int rangeCount;//行数
        private int columnCount;//列数
        private bool isOk;

        //private bool contractorClear = true;//是否清空承包方

        private SortedList<string, string> existPersons;
        private SortedList<string, string> existTablePersons;

        private string lastRowText = "合计";//最后一行第一个单元格中文字
        private string fileName = string.Empty;//文件名称
        private int currentIndex = 0;//当前索引号
        private int currentNumber = 0;//当前编号

        //3.14需求
        private string concordNumber = string.Empty;//合同编号

        private string bookNumber = string.Empty;//证书编号


        private List<YuLinTu.Library.Entity.Dictionary> dictList;
        private List<string> errorArray = new List<string>();//错误信息
        private List<string> warnArray = new List<string>();//警告信息
        private int originalValue = -1;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 导入excel表的名称
        /// </summary>
        public string ExcelName { get; set; }

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
            get { return currentZone; }
        }

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DbContext
        {
            set { dbContext = value; }
            get { return dbContext; }
        }


        public CollectivityTissue Tissue { get; set; }

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

        private List<LandFamily> _landFamilyCollection = new List<LandFamily>();

        #endregion Propertys

        #region Ctor

        public InitalizeLandTiesTable()
        {

        }

        #endregion Ctor

        #region Method

        #region Method - public

        public bool ReadTableInformation()
        {
            bool isAdd = false;
            int concordIndex = 1;
            if (!CheckValue())
            {
                return false;
            }
            var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vps = vpStation.GetByZoneCode(CurrentZone.FullCode);
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            var contractLands = landBusiness.GetLandCollection(CurrentZone.FullCode);

            dictList = new DictionaryBusiness(DbContext).GetAll();

            LandFamily landFamily = null;
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
                isOk = true;
                InitalizeSender();
                for (int index = calIndex; index < rangeCount; index++)
                {
                    currentIndex = index;//当前行数
                    string rowValue = GetString(allItem[index, 0]);//编号栏数据
                    if (rowValue.Trim() == lastRowText || rowValue.Trim() == "总计" || rowValue.Trim() == "共计")
                    {
                        if (!SetFamilyInfo(landFamily))
                            isOk = false;
                        break;
                    }
                    string familyName = GetString(allItem[currentIndex, 1]);
                    string change = GetString(allItem[currentIndex, 34]);
                    if (!string.IsNullOrEmpty(rowValue) && originalValue != int.Parse(rowValue))
                    {
                        if (!SetFamilyInfo(landFamily))
                            isOk = false;
                        originalValue = int.Parse(rowValue);
                        landFamily = NewFamily(rowValue, currentIndex, concordIndex);//重新创建
                        landFamily.CurrentFamily.Name = familyName;
                        var expand = landFamily.CurrentFamily.FamilyExpand;
                        expand.ChangeComment = change;
                        landFamily.CurrentFamily.FamilyExpand = expand;
                        AddLandFamily(landFamily);
                        concordIndex++;
                    }

                    if (string.IsNullOrEmpty(rowValue) && !string.IsNullOrEmpty(familyName))
                    {
                        ReportErrorInfo(this.ExcelName + string.Format("表中第{0}行承包方编号未填写内容!", index));
                        continue;
                    }
                    GetExcelInformation(landFamily, contractLands, vps);//获取Excel表中信息
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


        private bool SetFamilyInfo(LandFamily landFamily)
        {
            if (landFamily != null)
            {
                if (landFamily.CurrentFamily.FamilyExpand.ContractorType == eContractorType.Farmer)
                {
                    landFamily.CurrentFamily.PersonCount = landFamily.Persons.Count.ToString();
                    landFamily.CurrentFamily.SharePersonList = landFamily.Persons.ToList();
                }
                else
                {
                    landFamily.CurrentFamily.PersonCount = "0";
                    landFamily.CurrentFamily.SharePersonList = new List<Person>();
                }
                if (landFamily.CurrentFamily.FamilyExpand.ContractorType == eContractorType.Farmer &&
                    (landFamily.CurrentFamily.SourceID == null || landFamily.CurrentFamily.SourceID.Value == Guid.Empty))
                {
                    AddErrorMessage(this.ExcelName + string.Format("表中名称为 {0} 的承包方下家庭成员中不包含户主信息!", landFamily.CurrentFamily.Name));
                    return false;
                }
            }
            return true;
        }
        #endregion Method - public

        #region Method - private

        private LandFamily NewFamily(string rowValue, int currentIndex, int concordIndex)
        {
            int.TryParse(rowValue, out currentNumber);//当前编号
            var landFamily = new LandFamily();
            landFamily.CurrentFamily.VirtualType = eVirtualPersonType.Family;
            landFamily.Number = currentNumber;
            return landFamily;
        }

        private bool AddLandFamily(LandFamily landFamily)
        {
            existPersons = new SortedList<string, string>();
            existTablePersons = new SortedList<string, string>();
            LandFamilyCollection.Add(landFamily);
            return true;
        }

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

        private void GetExcelInformation(LandFamily landFamily, List<ContractLand> contractLands, List<VirtualPerson> vps)
        {
            InitalizeFamilyInformation(landFamily, vps);
            InitalizeLandInformation(landFamily, contractLands);
        }

        private void InitalizeFamilyInformation(LandFamily landFamily, List<VirtualPerson> vps)
        {
            var vpNumber = GetString(allItem[currentIndex, 3]);
            if (!string.IsNullOrEmpty(vpNumber))
            {
                var vpCode = CurrentZone.FullCode.PadRight(14, '0');
                vpNumber = vpNumber.Remove(0, vpCode.Length);
                if (vpNumber == "")
                {

                }
                var vp = vps.FirstOrDefault(f => f.FamilyNumber == vpNumber);
                if (vp != null)
                {
                    landFamily.CurrentFamily = vp;
                }
                else
                {
                    landFamily.CurrentFamily.ID = landFamily.CurrentFamily.ID;
                    landFamily.CurrentFamily.FamilyNumber = vpNumber;
                }

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
                landFamily.CurrentFamily.Address = GetString(allItem[currentIndex, 5]);

                string familyName = GetString(allItem[currentIndex, 1]);
                if (!string.IsNullOrEmpty(familyName))
                {
                    landFamily.CurrentFamily.Name = familyName;
                }
                string typestring = GetString(allItem[currentIndex, 2]);
                var expand = landFamily.CurrentFamily.FamilyExpand;
                if (typestring == "单位")
                {
                    expand.ContractorType = eContractorType.Unit;
                    expand.ConstructMode = eConstructMode.OtherContractType;
                }
                if (typestring == "个人")
                {
                    expand.ContractorType = eContractorType.Personal;
                    expand.ConstructMode = eConstructMode.OtherContractType;
                }
                landFamily.CurrentFamily.FamilyExpand = expand;
            }
            AddPerson(landFamily);
        }

        private void RecordErrorInformation(string errorInfo)
        {
            isOk = false;
            AddErrorMessage(errorInfo);
        }

        private void InitalizeLandInformation(LandFamily landFamily, List<ContractLand> contractLands)
        {
            List<Dictionary> listDLDJ = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            List<Dictionary> listTDYT = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
            List<Dictionary> listDKLYLX = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX);
            List<Dictionary> listDKLB = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            var landNumber = GetString(allItem[currentIndex, 16]);
            if (landNumber != "")
            {
                var entity = contractLands.Where(x => x.LandNumber == landNumber).FirstOrDefault();
                if (entity == null)
                {
                    if (contractLands.Count > 0)
                    {
                        entity = contractLands[0].Clone() as ContractLand;
                        entity.ID = Guid.NewGuid();
                        entity.ConcordId = null;
                        entity.Shape = null;
                        entity.ActualArea = 0;
                        entity.LandNumber = landNumber;
                    }
                    else
                    {
                        entity = new ContractLand() { LandNumber = landNumber };
                    }
                }

                entity.Name = GetString(allItem[currentIndex, 15]);
                entity.OwnRightType = GetString(allItem[currentIndex, 17]);
                entity.LandCategory = GetString(allItem[currentIndex, 18]);
                entity.LandCode = GetString(allItem[currentIndex, 19]);
                var dj = GetString(allItem[currentIndex, 20]);
                var yt = GetString(allItem[currentIndex, 21]);
                entity.LandLevel = listDLDJ.Find(c => c.Name == dj || c.Code == dj)?.Code;
                entity.Purpose = listTDYT.Find(c => c.Name == yt || c.Code == yt)?.Code;
                entity.LandCategory = listDKLB.Find(c => c.Name == entity.LandCategory || c.Code == entity.LandCategory)?.Code;
                entity.LandCode = listDKLYLX.Find(c => c.Name == entity.LandCode || c.Code == entity.LandCode)?.Code;
                entity.IsFarmerLand = (GetString(allItem[currentIndex, 22]) == "是") ? true : false;
                entity.TableArea = string.IsNullOrEmpty(GetString(allItem[currentIndex, 23])) ? 0 : Convert.ToDouble(GetString(allItem[currentIndex, 23]));
                entity.AwareArea = string.IsNullOrEmpty(GetString(allItem[currentIndex, 24])) ? 0 : Convert.ToDouble(GetString(allItem[currentIndex, 24]));
                entity.ActualArea = string.IsNullOrEmpty(GetString(allItem[currentIndex, 26])) ? 0 : Convert.ToDouble(GetString(allItem[currentIndex, 26]));
                entity.NeighborEast = GetString(allItem[currentIndex, 28]);
                entity.NeighborSouth = GetString(allItem[currentIndex, 29]);
                entity.NeighborWest = GetString(allItem[currentIndex, 30]);
                entity.NeighborNorth = GetString(allItem[currentIndex, 31]);
                entity.Comment = GetString(allItem[currentIndex, 32]);

                entity.OwnerName = landFamily.CurrentFamily.Name;
                entity.OwnerId = landFamily.CurrentFamily.ID;
                entity.ZoneCode = CurrentZone.FullCode;
                entity.ZoneName = CurrentZone.FullName;

                landFamily.LandCollection.Add(entity);
            }
        }

        private bool AddPerson(LandFamily landFamily)
        {
            string value = string.Empty;
            Person person = new Person();
            person.FamilyID = landFamily.CurrentFamily.ID;
            person.CreateTime = DateTime.Now;
            person.LastModifyTime = DateTime.Now;
            person.Nation = eNation.UnKnown;
            person.ZoneCode = currentZone.FullCode;
            //名称
            string name = GetString(allItem[currentIndex, 7]);
            person.Name = name;
            //性别
            value = GetString(allItem[currentIndex, 8]);
            if (string.IsNullOrEmpty(value))
            {
                person.Gender = eGender.Unknow;
            }
            else
            {
                person.Gender = GetGender(value);
            }
            person.Telephone = GetString(allItem[currentIndex, 9]);
            string CardType = GetString(allItem[currentIndex, 10]);

            person.CardType = GetCardType(CardType);
            //身份证号
            string icn = GetString(allItem[currentIndex, 11]);
            //家庭关系
            person.Relationship = GetString(allItem[currentIndex, 12]);
            //备注
            person.Comment = GetString(allItem[currentIndex, 13]);
            person.Opinion = GetString(allItem[currentIndex, 14]);
            //名称
            if (string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(icn)))
            {
                AddWarnMessage(this.ExcelName + string.Format("表序号为{0}的家庭成员姓名为空!", currentIndex + 1));
            }
            person.Name = name;

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
            if (person.Gender == eGender.Unknow && !string.IsNullOrEmpty(person.Name))
            {
                AddWarnMessage(this.ExcelName + "表中" + person.Name + "的性别填写不正确!");
            }

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
                landFamily.Persons.Add(person);
            }
            if (person.Name == landFamily.CurrentFamily.Name)
            {
                landFamily.CurrentFamily.SourceID = person.ID;
                landFamily.CurrentFamily.Number = person.ICN;
                landFamily.CurrentFamily.CardType = person.CardType;
                landFamily.CurrentFamily.Telephone = GetString(allItem[currentIndex, 4]);
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

        private eGender GetGender(string value)
        {
            if (value == "男性")
                return eGender.Male;

            if (value == "女性")
                return eGender.Female;
            return eGender.Unknow;
        }

        private bool CheckValue()
        {
            if (!OpenExcel())
                return false;
            if (!SetValue())
                return false;
            return true;
        }

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

        private bool ReportErrorInfo(string message)
        {
            isOk = false;
            AddErrorMessage(message);
            return false;
        }

        private void AddErrorMessage(string message)
        {
            if (errorArray == null)
                errorArray = new List<string>();
            if (!errorArray.Contains(message))
                errorArray.Add(message + "\n");
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

        private eCredentialsType GetCardType(string cardType)
        {
            switch (cardType)
            {
                case "1":
                    return eCredentialsType.IdentifyCard;

                case "2":
                    return eCredentialsType.OfficerCard;
                case "3":
                    return eCredentialsType.AgentCard;
                case "4":
                    return eCredentialsType.ResidenceBooklet;
                case "5":
                    return eCredentialsType.Passport;
                case "9":
                    return eCredentialsType.Other;

            }
            return eCredentialsType.IdentifyCard;
        }

        private void InitalizeSender()
        {
            var tissueWorkstation = dbContext.CreateSenderWorkStation();
            Tissue = tissueWorkstation.Get(CurrentZone.ID);
            if (Tissue == null)
            {
                Tissue = new CollectivityTissue();
            }
            Tissue.Name = GetString(allItem[2, 2]);
            Tissue.Code = GetString(allItem[2, 6]);
            Tissue.LawyerName = GetString(allItem[2, 9]);
            eCredentialsType cardtype = GetCardType(GetString(allItem[2, 9]));
            Tissue.LawyerCredentType = cardtype;
            Tissue.LawyerCartNumber = GetString(allItem[2, 15]);
            Tissue.LawyerTelephone = GetString(allItem[2, 20]);
            Tissue.LawyerAddress = GetString(allItem[2, 25]);
            Tissue.LawyerPosterNumber = GetString(allItem[2, 29]);
            Tissue.SurveyPerson = GetString(allItem[2, 31]);
            Tissue.SurveyDate = GetDateTime(allItem[2, 34]);
        }

        #endregion Method - private

        #region Method - override

        public override void Read()

        {
        }

        public override void Write()
        {
        }

        #endregion Method - override

        #endregion Method
    }
}