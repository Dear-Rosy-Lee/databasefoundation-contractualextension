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
    public class ImportLandTiesTable : Task
    {
        #region Fields

        private ToolProgress toolProgress;
        private List<string> concrdNumberList;
        private int familyCount;//承包方数
        private int personCount;//共有人数
        private int landCount;//地块数
        private bool isOk;//导入地块时的验证
        private CollectivityTissue tissue;//集体经济组织
        private CollectivityTissue sender;//集体经济组织
        private VirtualPersonBusiness personBusiness;
        private AccountLandBusiness landBusiness;
        private DictionaryBusiness dictBusiness;
        private ConcordBusiness concordBusiness;
        private ContractRegeditBookBusiness contractRegeditBookBusiness;
        private IBuildLandBoundaryAddressCoilWorkStation coilStation;
        private IBuildLandBoundaryAddressDotWorkStation dotStation;

        private List<VirtualPerson> remainVps = new List<VirtualPerson>();
        private List<ContractLand> remainLands = new List<ContractLand>();
        private List<ContractConcord> remainConcords = new List<ContractConcord>();
        private List<ContractRegeditBook> remainBooks = new List<ContractRegeditBook>();

        private List<Dictionary> dictList = new List<Dictionary>();  //数据字典集合
        private ContractBusinessSettingDefine ContractBusinessSettingDefine = ContractBusinessSettingDefine.GetIntence();
        private int rangeCount;//行数
        private int columnCount;//列数
        private int currentIndex = 0;//当前索引号
        private object[,] allItem;
        private string lastRowText = "合计";//最后一行第一个单元格中文字
        private InitalizeLandTiesTable landInfo;//初始化承包台账调查表信息

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 是否验证地块编码重复
        /// </summary>
        public bool IsCheckLandNumberRepeat { get; set; }

        /// <summary>
        /// 是否清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// Excel文件名称
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 每个地域占的百分比跨度
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 当前地域下的所有承包方
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        #endregion Propertys

        #region Ctor

        public ImportLandTiesTable()
        {
            isOk = true;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
        }

        #endregion Ctor

        #region Methods

        #region Methods - Public

        /// <summary>
        /// 读取户籍信息
        /// </summary>
        public bool ReadLandTableInformation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            personBusiness = new VirtualPersonBusiness(DbContext);
            landBusiness = new AccountLandBusiness(DbContext);
            concordBusiness = new ConcordBusiness(DbContext);
            contractRegeditBookBusiness = new ContractRegeditBookBusiness(DbContext);
            coilStation = DbContext.CreateBoundaryAddressCoilWorkStation();
            dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
            dictBusiness = new DictionaryBusiness(DbContext);   //数据字典
            dictList = dictBusiness.GetAll();
            if (dictList == null || dictList.Count == 0)
            {
                this.ReportErrorInfo("数据字典为空，请检查");
                return false;
            }
            landInfo = new InitalizeLandTiesTable();
            landInfo.CurrentZone = CurrentZone;
            landInfo.DbContext = DbContext;
            landInfo.FileName = fileName;
            landInfo.ExcelName = ExcelName;
            landInfo.TableType = TableType;

            bool success = landInfo.ReadTableInformation();
            return success;
        }

        public void ImportLandEntity()
        {
            foreach (LandFamily landFamily in landInfo.LandFamilyCollection)
            {
                int familyIndex = 1;
                UpdateVirtualPerson(landFamily, familyIndex);
                List<ContractLand> temp = landFamily.LandCollection.Clone() as List<ContractLand>;
                if (temp.Count != 0)
                {
                    var landStation = DbContext.CreateContractLandWorkstation();
                    landStation.DeleteLandByPersonID(landFamily.CurrentFamily.ID);
                    UpdateContractLand(temp);//导入地块
                }
            }
        }

        #endregion Methods - Public

        #region Methods - Private

        private void UpdateContractLand(List<ContractLand> lands)
        {
            foreach (var land in lands)
            {
                //land.ConcordId = null;

                CheckSurveyNumber(land);   //检查同村下调查编码

                if (remainLands.Any(c => !string.IsNullOrEmpty(c.LandNumber) && c.LandNumber == land.LandNumber))
                {
                    //TODO 逻辑待考虑
                    var temp = remainLands.Find(c => c.ID == land.ID);
                    if (temp == null)
                    {
                        landBusiness.AddLand(land);
                        landCount++;
                        continue;
                    }
                    if (!string.IsNullOrEmpty(temp.LocationCode)
                       && (temp.LocationCode == CurrentZone.UpLevelCode || temp.LocationCode == CurrentZone.FullCode))
                    {
                        landBusiness.Delete(temp.ID);
                    }
                    else
                    {
                        this.ReportErrorInfo("Excel中承包方" + land.OwnerName + "的地块地块编码号:" + ContractLand.GetLandNumber(land.CadastralNumber) + "与" +
                            temp.LocationName + "下承包方" + temp.OwnerName + " 的地块地块编码重复");
                        isOk = false;
                        return;
                    }
                }

                //if (landBusiness.IsLandNumberReapet(land.LandNumber, land.ID, CurrentZone.FullCode))  //根据地籍号查找是否存在
                //{
                //    ContractLand temp = landBusiness.GetLandById(land.ID);    //得到承包地块
                //    if (temp == null)
                //    {
                //        landBusiness.AddLand(land);
                //        landCount++;
                //        continue;
                //    }
                //    if (!string.IsNullOrEmpty(temp.LocationCode)
                //        && (temp.LocationCode == CurrentZone.UpLevelCode || temp.LocationCode == CurrentZone.FullCode))
                //    {
                //        landBusiness.Delete(temp.ID);
                //    }
                //    else
                //    {
                //        this.ReportErrorInfo("Excel中户 " + land.OwnerName + "的地块地块编码号:" + ContractLand.GetLandNumber(land.CadastralNumber) + "与" +
                //            temp.LocationName + "下 户" + temp.OwnerName + " 的地块地块编码重复");
                //        isOk = false;
                //        return;
                //    }
                //}

                land.Name = land.Name.Replace("\0", "");
                land.LandNumber = $"{land.LandNumber}";
                if (land.Comment.Contains("自留地"))
                    land.LandCategory = "21";
                landBusiness.AddLand(land);
                landCount++;
            }
        }

        private void CheckSurveyNumber(ContractLand land)
        {
            if (CurrentZone.Level == eZoneLevel.Group)
            {
                string landNumber = land.LandNumber;  // ContractLand.GetLandNumber(land.CadastralNumber);
                //List<ContractLand> landCollection = new List<ContractLand>();//DbContext.ContractLand.SL_GetCollection("CadastralNumber", CurrentZone.UpLevelCode, Data.ConditionOption.Like_LeftFixed);
                List<ContractLand> lands = remainLands.FindAll(ld => ld.CadastralNumber.Substring(ld.CadastralNumber.Length > 19 ? 19 : 0) == landNumber);
                if (lands != null && lands.Count > 0)
                {
                    foreach (ContractLand conLand in lands)
                    {
                        if (conLand.CadastralNumber == land.CadastralNumber)
                        {
                            continue;
                        }
                        this.ReportExcetionInfo("地块编码" + landNumber + "在" + conLand.LocationName + "下已经存在!");
                    }
                }
            }
        }

        private bool ReportExcetionInfo(string message)
        {
            this.ReportWarn(message);
            return false;
        }

        /// <summary>
        /// 导入承包方
        /// </summary>
        private VirtualPerson UpdateVirtualPerson(LandFamily landFamily, int familyIndex)
        {
            //生成承包方中的共有人信息
            List<Person> personList = new List<Person>();
            foreach (Person per in landFamily.Persons)
            {
                if (per == null)
                {
                    continue;
                }
                if (per.Name == landFamily.CurrentFamily.Name) // 判断是否为户主
                {
                    landFamily.CurrentFamily.Number = per.ICN;
                    landFamily.CurrentFamily.CardType = per.CardType;
                }
                personList.Add(per);
            }
            landFamily.CurrentFamily.SharePersonList = personList;
            landFamily.CurrentFamily.ZoneCode = CurrentZone.FullCode;
            landFamily.CurrentFamily.VirtualType = eVirtualPersonType.Family;

            if (string.IsNullOrEmpty(landFamily.CurrentFamily.Address))
            {
                landFamily.CurrentFamily.Address = CurrentZone.FullName;
            }
            landFamily.CurrentFamily.CreationTime = DateTime.Now;
            landFamily.CurrentFamily.PersonCount = landFamily.Persons != null ? landFamily.Persons.Count.ToString() : "";

            int result = -2;
            VirtualPerson vp = null;
            try
            {
                var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                //List<VirtualPerson> vps = personBusiness.GetCollection(CurrentZone.FullCode, landFamily.CurrentFamily.Name);     //从数据库中获取该承包方名称的数据
                List<VirtualPerson> vps = remainVps == null ? new List<VirtualPerson>() : remainVps.FindAll(c => c.Name == landFamily.CurrentFamily.Name);
                if (vps == null || vps.Count == 0)
                {
                    vp = landFamily.CurrentFamily;
                    familyCount++;
                    personCount += landFamily.CurrentFamily.SharePersonList.Count(); //统计共有人人数
                    result = personStation.Update(vp);
                }
                else
                {
                    vp = vps.FirstOrDefault();  // vps[0];
                }
                personList = null;
                vps = null;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ImportVirtualPerson(导入承包方数据失败!)", ex.Message + ex.StackTrace);
                throw new YltException("导入承包方数据失败!");
            }
            return (result == -2 || result > 0) ? vp : null;
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        private bool ReportErrorInfo(string message)
        {
            this.ReportError(message);
            return false;
        }

        #endregion Methods - Private

        #endregion Methods
    }
}