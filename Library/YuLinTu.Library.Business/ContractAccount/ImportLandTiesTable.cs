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
        private int familyCount;//承包方数
        private int personCount;//共有人数
        private int landCount;//地块数
        
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

        //导入地块时的验证
        public bool isOk { get; set; }

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
            try
            {
                DbContext.BeginTransaction();

                ClearLandReleationData();    //清空数据

                //获取目标数据源中当前地域下的所有户信息，用于之后的快速判断，减少数据库的交互次数
                toolProgress.InitializationPercent(landInfo.LandFamilyCollection.Count, Percent, CurrentPercent);

                isOk = true;
                int familyIndex = 1;
                Zone zone = CurrentZone.Level == eZoneLevel.Village ? CurrentZone.Clone() as Zone : GetParent(CurrentZone);
                tissue = concordBusiness.GetSenderById(zone.ID);
                sender = concordBusiness.GetSenderById(CurrentZone.ID);
                var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = DbContext.CreateContractLandWorkstation();
                var concordStation = DbContext.CreateConcordStation();
                var bookStation = DbContext.CreateRegeditBookStation();
                var senderStation = DbContext.CreateSenderWorkStation();
                remainVps = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                remainLands = landStation.GetCollection(CurrentZone.FullCode, eLevelOption.Self);
                remainConcords = concordStation.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                remainBooks = bookStation.GetByZoneCode(CurrentZone.FullCode, eSearchOption.Fuzzy);
                senderStation.Update(landInfo.Tissue);
                foreach (LandFamily landFamily in landInfo.LandFamilyCollection)
                {
                    ImportLandFamily(landFamily, familyIndex);        //导入承包地、承包方
                    familyIndex++;

                    string info = string.Format("导入承包方{0}", landFamily.CurrentFamily.Name);
                    toolProgress.DynamicProgress(info);
                }
                if (familyCount == landInfo.LandFamilyCollection.Count)
                {
                    this.ReportInfomation(string.Format("{0}表中共有{1}户承包方数据,成功导入{2}户承包方记录、{3}条共有人记录、{4}宗地块记录!", ExcelName, landInfo.LandFamilyCollection.Count, landInfo.LandFamilyCollection.Count, personCount, landCount));
                }
                else
                {
                    this.ReportInfomation(string.Format("{0}表中共有{1}户承包方数据,成功导入{2}户承包方记录、{3}条共有人记录、{4}宗地块记录,其中{5}户承包方数据被锁定!", ExcelName, landInfo.LandFamilyCollection.Count, familyCount, personCount, landCount, landInfo.LandFamilyCollection.Count - familyCount));
                }
                DbContext.CommitTransaction();
                zone = null;
                tissue = null;
            }
            catch (Exception ex)
            {
                DbContext.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportLandEntity(导入地籍调查表失败!)", ex.Message + ex.StackTrace);
            }
            finally
            {
                landInfo.Dispose();
            }
            //foreach (LandFamily landFamily in landInfo.LandFamilyCollection)
            //{
            //    int familyIndex = 1;
            //    UpdateVirtualPerson(landFamily, familyIndex);
            //    List<ContractLand> temp = landFamily.LandCollection.Clone() as List<ContractLand>;
            //    if (temp.Count != 0)
            //    {
            //        var landStation = DbContext.CreateContractLandWorkstation();
            //        landStation.DeleteLandByPersonID(landFamily.CurrentFamily.ID);
            //        UpdateContractLand(temp);//导入地块
            //    }
            //}
        }

        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        #endregion Methods - Public

        #region Methods - Private

        private VirtualPerson ImportVirtualPerson(LandFamily landFamily, int familyIndex)
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
                    result = personStation.Add(vp);
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

        private VirtualPerson ImportVirtualPersonInformation(LandFamily landFamily, int familyIndex)
        {
            bool contractorClear = ContractBusinessSettingDefine.ClearVirtualPersonData;
            VirtualPerson vp = null;
            try
            {
                if (contractorClear)
                {
                    //导入承包方(此时未被锁定的承包方已被清空,直接导入承包方数据)
                    vp = ImportVirtualPerson(landFamily, familyIndex);
                    if (vp == null)
                    {
                        ReportErrorInfo("导入承包方信息时发生错误。");
                        return null;
                    }
                    if (vp != null && vp.Status == eVirtualPersonStatus.Lock)
                    {
                        ReportExcetionInfo(string.Format("承包方{0}被锁定,略过!", vp.Name));
                        return null;
                    }
                }
                else
                {
                    //获取承包方信息
                    //vp = personBusiness.Get(landFamily.CurrentFamily.Name, CurrentZone.FullCode);
                    vp = remainVps.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == landFamily.CurrentFamily.Name);
                    if (vp == null)
                    {
                        this.ReportExcetionInfo(string.Format("承包方{0}未匹配,略过!", landFamily.CurrentFamily.Name));
                        return null;
                    }
                    if (vp != null && vp.Status == eVirtualPersonStatus.Lock)
                    {
                        this.ReportExcetionInfo(string.Format("承包方{0}被锁定,略过!", landFamily.CurrentFamily.Name));
                        return null;
                    }
                    int sourceNumber = -1;
                    int compareNumber = -1;
                    Int32.TryParse(vp.FamilyNumber, out sourceNumber);
                    Int32.TryParse(landFamily.CurrentFamily.FamilyNumber, out compareNumber);
                    if (sourceNumber != compareNumber)
                    {
                        this.ReportExcetionInfo(string.Format("承包方{0}编号:{1}与承包方调查表中{2}编号：{3}不一致!", vp.Name, sourceNumber, landFamily.CurrentFamily.Name, compareNumber));
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ImportVirtualPersonInformation(导入承包方数据失败!)", ex.Message + ex.StackTrace);
                throw new YltException("导入承包方数据失败!");
            }
            return vp;
        }

        private List<ContractLand> CombinationLand(List<ContractLand> lands, VirtualPerson vp)
        {
            if (lands == null || lands.Count < 1)
                return lands;

            //把所有承包地添加到目标数据源中
            for (int i = 0; i < lands.Count; i++)
            {
                if (lands[i] == null)
                    continue;

                lands[i].LocationCode = CurrentZone.FullCode;
                lands[i].LocationName = CurrentZone.FullName;

                //设置地块的权属单位
                //如果存在相应的集体经济组织则其权属单位为对应集体经济组织，否则为当前地域（地域是默认的集体经济组织）
                //CollectivityTissue tissue = concordBusiness.GetSenderById(CurrentZone.ID);
                if (sender != null)
                {
                    lands[i].ZoneCode = sender.Code;
                    lands[i].ZoneName = sender.Name;
                }
                else
                {
                    lands[i].ZoneCode = CurrentZone.FullCode;
                    lands[i].ZoneName = CurrentZone.FullName;
                }

                lands[i].Founder = "Admin";
                lands[i].CreationTime = DateTime.Now;

                //设置承包地的承包方
                lands[i].OwnerId = vp.ID;
                lands[i].OwnerName = vp.Name;
            }

            return lands;
        }

        private void ImportLandFamily(LandFamily landFamily, int familyIndex)
        {
            VirtualPerson vp = ImportVirtualPersonInformation(landFamily, familyIndex);
            if (vp == null)
            {
                return;
            }

            //导入合同信息陈泽林修改20160901
            //if (landFamily.Concord != null && (landFamily.Concord.CountActualArea > 0 || landFamily.Concord.CountAwareArea > 0
            //    || landFamily.Concord.CountMotorizeLandArea > 0 || landFamily.Concord.TotalTableArea > 0
            //    || !string.IsNullOrEmpty(landFamily.Concord.ConcordNumber) || !string.IsNullOrEmpty(landFamily.Concord.ContractCredentialNumber)))
            //{
            //    ImportConcordAndRegeditBook(landFamily, vp);//导入合同及证书
            //}

            //导入地块数据
            landFamily.LandCollection = CombinationLand(landFamily.LandCollection, vp);   //组合地块数据
            foreach (ContractLand land in landFamily.LandCollection)
            {
                //if (!string.IsNullOrEmpty(land.LandNumber) && land.LandNumber.Length >= 5)
                //{
                land.SurveyNumber = land.CadastralNumber; //.Substring(land.LandNumber.Length - 5);
                //}//InitalizeAgricultureLandShare(land);
                EnumNameAttribute[] values = EnumNameAttribute.GetAttributes(typeof(eLandCategoryType));
                for (int i = 0; i < values.Length; i++)    //通过地块的备注给地块类别赋值
                {
                    int index = land.Comment.IndexOf("(" + values[i].Description + ")");
                    if (index < 0)
                    {
                        index = land.Comment.IndexOf("（" + values[i].Description + "）");
                    }
                    if (index < 0)
                    {
                        continue;
                    }
                    //string objValue = this.dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB).Find(c => c.Name == values[i].Value.ToString()).Code;
                    string objValue = values[i].Value == null ? "" : ((int)values[i].Value).ToString();
                    land.LandCategory = objValue;
                    break;
                }
            }
            List<ContractLand> temp = landFamily.LandCollection.Clone() as List<ContractLand>;
            ImportContractLand(temp);//导入地块

            vp = null;
        }

        private void ImportContractLand(List<ContractLand> lands)
        {
            bool checkNumber = false;
            string value = ToolConfiguration.GetSpecialAppSettingValue("CheckInnerVillageNumber", "false");
            Boolean.TryParse(value, out checkNumber);
            foreach (var land in lands)
            {
                //land.ConcordId = null;
                if (checkNumber)
                {
                    CheckSurveyNumber(land);   //检查同村下调查编码
                }
                if (remainLands.Any(c => !string.IsNullOrEmpty(c.LandNumber) && c.LandNumber == land.LandNumber))
                {
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

        public void ClearLandReleationData()
        {
            int familyCount = personBusiness.CountByZone(CurrentZone.FullCode);
            int landCount = landBusiness.CountLandByZone(CurrentZone.FullCode);
            if (familyCount <= 0 && landCount <= 0)
            {
                return;
            }
            //List<VirtualPerson> unLockFamilys = personBusiness.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
            //List<VirtualPerson> familys = personBusiness.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
            //List<VirtualPerson> unLockFamilys = landBusiness.GetByZone(CurrentZone.FullCode);
            try
            {
                //unLockFamilys = FilterContractor(familys, unLockFamilys);
                //ClearLandInformtion(familys, unLockFamilys);
                DeleteAllLandDataByZone(ContractBusinessSettingDefine.ClearVirtualPersonData);
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearLandReleationData(清除地籍数据失败)", ex.Message + ex.StackTrace);
                throw new YltException("清空地籍数据失败!"); ;
            }
            finally
            {
                //familys = null;
                //unLockFamilys = null;
                GC.Collect();
            }
        }

        private bool DeleteAllLandDataByZone(bool vpClear)
        {
            bool isDeleteSuccess = true;
            try
            {
                var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = DbContext.CreateContractLandWorkstation();
                var concordStation = DbContext.CreateConcordStation();
                var bookStation = DbContext.CreateRegeditBookStation();
                var dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = DbContext.CreateBoundaryAddressCoilWorkStation();
                if (vpClear)
                    personStation.DeleteByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);

                landStation.DeleteOtherByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                concordStation.DeleteOtherByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                bookStation.DeleteByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                dotStation.DeleteByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                coilStation.DeleteByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
            }
            catch (Exception ex)
            {
                isDeleteSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteAllLandDataByZone(清除数据失败)", ex.Message + ex.StackTrace);
            }
            return isDeleteSuccess;
        }

        private void UpdateContractLand(List<ContractLand> lands)
        {
            foreach (var land in lands)
            {
                //land.ConcordId = null;

                CheckSurveyNumber(land);   //检查同村下调查编码

                if (remainLands.Any(c => !string.IsNullOrEmpty(c.LandNumber) && c.LandNumber == land.LandNumber))
                {
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