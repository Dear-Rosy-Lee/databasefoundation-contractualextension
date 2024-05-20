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
    public class InitalizeLandTiesTableLZ : ExcelBase
    {
        #region Fields

        private const double PRECISION = 0.000001;//精度
        private Zone currentZone;
        private IDbContext dbContext;
        private object[,] allItem;
        private int rangeCount;//行数
        private int columnCount;//列数
        private bool isOk;
        private int landCount = 0;
        private bool contractorClear = true;//是否清空承包方
        private List<Dictionary> dictDKLB;
        private List<Dictionary> dictTDYT;
        private List<Dictionary> dictDLDJ;
        private List<Dictionary> dicSF;
        private SortedList<string, string> existPersons;
        private SortedList<string, string> existTablePersons;

        private string lastRowText = "合计";//最后一行第一个单元格中文字
        private List<string> _files;
        public string FileName;//文件名称
        private int currentIndex = 0;//当前索引号
        private int currentNumber = 0;//当前编号
        private List<ContractLand> contractLands;

        //3.14需求
        private string concordNumber = string.Empty;//合同编号

        private string bookNumber = string.Empty;//证书编号

        private List<string> errorArray = new List<string>();//错误信息
        private List<string> warnArray = new List<string>();//警告信息
        private int originalValue = 0;

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
        public List<string> Files
        {
            set { _files = value; }
            get { return _files; }
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

        public InitalizeLandTiesTableLZ()
        {
        }

        #endregion Ctor

        #region Method

        #region Method - public

        public bool ReadTableInformation()
        {
            var dictStation = DbContext.CreateDictWorkStation();
            var DictList = dictStation.Get();
            if (DictList != null && DictList.Count > 0)
            {
                dicSF = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.SF);
                dictDKLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
                dictTDYT = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
                dictDLDJ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DLDJ);
            }
            LandFamilyCollection = new List<LandFamily>();
            foreach (var file in Files)
            {
                FileName = file;
                if (!CheckValue())
                {
                    return false;
                }

                LandFamily landFamily = new LandFamily();
                try
                {
                    landCount = GetLandIndex();//获取地块数量

                    existPersons = new SortedList<string, string>();
                    existTablePersons = new SortedList<string, string>();
                    isOk = true;
                    string familyName = GetString(allItem[1, 4]);
                    var number = GetString(allItem[1, 10]);
                    number = number.Substring(number.Length - 5, 4);
                    landFamily.CurrentFamily.FamilyNumber = number;

                    GetExcelInformation(landFamily);//获取Excel表中信息
                }
                catch (Exception ex)
                {
                    Dispose();
                    YuLinTu.Library.Log.Log.WriteException(this, "ReadTableInformation(读取表格信息失败)", ex.Message + ex.StackTrace);
                    return ReportErrorInfo(this.ExcelName + "转换过程中发生错误：" + ex.Message.ToString() + ",请确认导入数据是否完整匹配!");
                }
                LandFamilyCollection.Add(landFamily);
            }
            List<ContractLand> lands = new List<ContractLand>();
            List<ContractLand> emptylands = new List<ContractLand>();
            //处理空地块编码
            foreach (var family in LandFamilyCollection)
            {
                family.LandCollection.ForEach(x => { lands.Add(x); });
            }
            foreach (var family in LandFamilyCollection)
            {
                foreach (var land in family.LandCollection)
                {
                    if (land.LandNumber == "")
                    {
                        var maxLandNumber = lands.Select(x => x.LandNumber).Max();
                        maxLandNumber = maxLandNumber.Substring(maxLandNumber.Length - 5);
                        var number = int.Parse(maxLandNumber) + 1;
                        land.LandNumber = CurrentZone.FullCode + number.ToString().PadLeft(5, '0');
                    }
                }
            }

            return isOk;
        }

        #endregion Method - public

        #region Method - private

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

        private int GetLandIndex()
        {
            int result = 0;
            for (int i = 11; i < rangeCount; i++)
            {
                var value = GetString(allItem[i, 0]);
                if (value == "")
                {
                    break;
                }
                result++;
            }
            return result;
        }

        private void GetExcelInformation(LandFamily landFamily)
        {
            try
            {
                InitalizeFamilyInformation(landFamily);

                InitalizeLandInformation(landFamily, contractLands);
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetExcelInformation(获取Excel表中信息失败)", ex.Message + ex.StackTrace);
                ReportErrorInfo("读取表格信息失败，" + string.Format("请检查导入地块调查表配置是否与{0}表结构匹配", this.ExcelName));
            }
        }

        private void InitalizeFamilyInformation(LandFamily landFamily)
        {
            string familyName = GetString(allItem[1, 4]);
            if (!string.IsNullOrEmpty(familyName))
            {
                landFamily.CurrentFamily.Name = familyName;
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
            for (int i = 11; i < landCount + 11; i++)
            {
                var landNumber = GetString(allItem[i, 0]);
                var landName = GetString(allItem[i, 1]);
                var landActualArea = GetString(allItem[i, 4]);
                if (landNumber != "")
                {
                    var entity = contractLands.Where(x => x.LandNumber == landNumber).FirstOrDefault();
                    if (entity == null)
                    {
                        entity = contractLands.Where(x => x.Name == landName && x.ActualArea.ToString() == landActualArea).FirstOrDefault();
                    }
                    if (entity != null)
                    {
                        entity.Name = GetString(allItem[i, 1]);
                        entity.TableArea = string.IsNullOrEmpty(GetString(allItem[i, 3])) ? 0 : Convert.ToDouble(GetString(allItem[i, 3]));
                        entity.ActualArea = string.IsNullOrEmpty(GetString(allItem[i, 4])) ? 0 : Convert.ToDouble(GetString(allItem[i, 4]));
                        entity.NeighborEast = GetString(allItem[i, 5]);
                        entity.NeighborSouth = GetString(allItem[i, 6]);
                        entity.NeighborWest = GetString(allItem[i, 7]);
                        entity.NeighborNorth = GetString(allItem[i, 8]);
                        Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(GetString(allItem[i, 9])) || c.Code.Equals(GetString(allItem[i, 9])));
                        Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(GetString(allItem[i, 10])) || c.Code.Equals(GetString(allItem[i, 10])));
                        //Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(GetString(allItem[i, 11])) || c.Code.Equals(GetString(allItem[i, 11])));
                        entity.LandCategory = dklb.Code;
                        entity.Purpose = tdyt.Code;
                        entity.LandLevel = GetString(allItem[i, 11]).PadLeft(2, '0');
                        entity.Comment = GetString(allItem[i, 12]);
                        landFamily.LandCollection.Add(entity);
                    }
                    else
                    {
                        ContractLand land = new ContractLand();
                        land.OwnerName = landFamily.CurrentFamily.Name;
                        land.OwnerId = landFamily.CurrentFamily.ID;
                        land.LocationCode = CurrentZone.FullCode;
                        land.LocationName = CurrentZone.FullName;
                        land.LandNumber = "";
                        land.Name = GetString(allItem[i, 1]);
                        land.TableArea = string.IsNullOrEmpty(GetString(allItem[i, 3])) ? 0 : Convert.ToDouble(GetString(allItem[i, 3]));
                        land.ActualArea = string.IsNullOrEmpty(GetString(allItem[i, 4])) ? 0 : Convert.ToDouble(GetString(allItem[i, 4]));
                        land.NeighborEast = GetString(allItem[i, 5]);
                        land.NeighborSouth = GetString(allItem[i, 6]);
                        land.NeighborWest = GetString(allItem[i, 7]);
                        land.NeighborNorth = GetString(allItem[i, 8]);
                        Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(GetString(allItem[i, 9])) || c.Code.Equals(GetString(allItem[i, 9])));
                        Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(GetString(allItem[i, 10])) || c.Code.Equals(GetString(allItem[i, 10])));
                        //Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(GetString(allItem[i, 11])) || c.Code.Equals(GetString(allItem[i, 11])));
                        land.LandCategory = dklb.Code;
                        land.Purpose = tdyt.Code;
                        land.LandLevel = GetString(allItem[i, 11]).PadLeft(2, '0');
                        land.Comment = GetString(allItem[i, 12]);
                        landFamily.LandCollection.Add(land);
                    }
                }
            }
        }

        private void AddPerson(LandFamily landFamily)
        {
            List<string> icns = new List<string>();

            for (var i = 3; i < 8; i++)
            {
                if (GetString(allItem[i, 1]) != "")
                {
                    Person person1 = new Person();
                    person1.FamilyID = landFamily.CurrentFamily.ID;
                    person1.CreateTime = DateTime.Now;
                    person1.LastModifyTime = DateTime.Now;
                    person1.Nation = eNation.UnKnown;
                    person1.ZoneCode = currentZone.FullCode;
                    person1.Name = GetString(allItem[i, 1]);
                    person1.Relationship = GetString(allItem[i, 2]);
                    person1.ICN = GetString(allItem[i, 3]);
                    icns.Add(GetString(allItem[i, 3]));
                    var sf = (GetString(allItem[i, 5]) == "有") ? "是" : GetString(allItem[i, 5]);
                    Dictionary SF = dicSF.Find(c => c.Name.Equals(sf));
                    person1.IsSharedLand = SF.Code;
                    person1.Comment = GetString(allItem[i, 6]);
                    landFamily.Persons.Add(person1);
                }

                if (GetString(allItem[i, 7]) != "")
                {
                    Person person2 = new Person();
                    person2.FamilyID = landFamily.CurrentFamily.ID;
                    person2.CreateTime = DateTime.Now;
                    person2.LastModifyTime = DateTime.Now;
                    person2.Nation = eNation.UnKnown;
                    person2.ZoneCode = currentZone.FullCode;
                    person2.Name = GetString(allItem[i, 7]);
                    person2.Relationship = GetString(allItem[i, 8]);
                    person2.ICN = GetString(allItem[i, 9]);
                    icns.Add(GetString(allItem[i, 9]));
                    person2.IsSharedLand = GetString(allItem[i, 11]);
                    person2.Comment = GetString(allItem[i, 12]);
                    landFamily.Persons.Add(person2);
                }
            }
            try
            {
                var landStation = DbContext.CreateContractLandWorkstation();
                var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var vps = vpStation.GetByZoneCode(CurrentZone.FullCode).ToList();
                var vp = vps.Where(x => x.Number == GetString(allItem[3, 3])).FirstOrDefault();
                if (vp == null)
                {
                    foreach (var icn in icns)
                    {
                        vp = vps.Where(x => x.Number == icn).FirstOrDefault();
                        if (vp != null)
                        {
                            break;
                        }
                    }
                    if (vp == null)
                    {
                        foreach (var item in vps)
                        {
                            Person person = new Person();
                            foreach (var icn in icns)
                            {
                                person = item.SharePersonList.Where(t => t.ICN == icn).FirstOrDefault();
                                if (person != null)
                                {
                                    vp = item;
                                    break;
                                }
                            }
                        }
                    }
                    if (vp == null)
                    {
                        RecordErrorInformation($"请检查导入文件夹中名字为：{landFamily.CurrentFamily.Name} 的数据。");
                    }
                }

                contractLands = new List<ContractLand>();
                if (vp != null)
                {
                    contractLands = landStation.GetCollection(vp.ID);
                }
            }
            catch (Exception ex)
            {
            }
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
                Open(FileName);
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
            var res = GetString(allItem[2, 34]);
            DateTime? date;
            if (res == "")
            {
                date = null;
            }
            else
            {
                date = Convert.ToDateTime(GetString(allItem[2, 34]));
            }
            Tissue.SurveyDate = date;
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