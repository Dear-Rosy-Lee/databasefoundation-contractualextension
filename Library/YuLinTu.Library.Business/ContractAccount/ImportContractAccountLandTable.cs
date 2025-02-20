/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入承包台账调查表地块数据
    /// </summary>
    [Serializable]
    public class ImportContractAccountLandTable : Task
    {
        #region Fields

        protected ToolProgress toolProgress;
        protected List<string> concrdNumberList;
        protected InitalizeLandSurveyInformation landInfo;//初始化承包台账调查表信息
        protected int familyCount;//承包方数
        protected int personCount;//共有人数
        protected int landCount;//地块数
        protected bool isOk;//导入地块时的验证
        protected CollectivityTissue tissue;//集体经济组织
        protected CollectivityTissue sender;//集体经济组织
        protected VirtualPersonBusiness personBusiness;
        protected AccountLandBusiness landBusiness;
        protected DictionaryBusiness dictBusiness;
        protected ConcordBusiness concordBusiness;
        protected ContractRegeditBookBusiness contractRegeditBookBusiness;
        protected IBuildLandBoundaryAddressCoilWorkStation coilStation;
        protected IBuildLandBoundaryAddressDotWorkStation dotStation; 
        protected List<VirtualPerson> remainVps = new List<VirtualPerson>();
        protected List<ContractLand> remainLands = new List<ContractLand>();
        protected List<ContractConcord> remainConcords = new List<ContractConcord>();
        protected List<ContractRegeditBook> remainBooks = new List<ContractRegeditBook>(); 
        protected List<Dictionary> dictList = new List<Dictionary>();  //数据字典集合
        protected ContractBusinessSettingDefine ContractBusinessSettingDefine = ContractBusinessSettingDefine.GetIntence();

        #endregion Fields
         
        #region Propertys

        //protected ContractBusinessSettingDefine _contractBusinessSettingDefine;

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefin = ContractBusinessSettingDefine.GetIntence();

        ///// <summary>
        ///// 承包台账导入配置
        ///// </summary>
        //public ContractBusinessImportSurveyDefine ContractLandImportSurveyDefine
        //{
        //    get { return ContractBusinessImportSurveyDefine.GetIntence(); }
        //}

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 是否验证地块编码重复 不要删除此属性  安徽插件使用
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

        public eVirtualType VirtualType { get; set; }

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

        public ImportContractAccountLandTable()
        {
            isOk = true;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        protected void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion Ctor

        #region Methods

        #region Methods - ReadInformation

        /// <summary>
        /// 读取户籍信息
        /// </summary>
        public bool ReadLandTableInformation(string fileName, bool isNotLand)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            personBusiness = new VirtualPersonBusiness(DbContext);
            personBusiness.VirtualType = this.VirtualType;
            landBusiness = new AccountLandBusiness(DbContext);
            landBusiness.VirtualType = this.VirtualType;
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
            landInfo = new InitalizeLandSurveyInformation(dictList);
            landInfo.CurrentZone = CurrentZone;
            landInfo.DbContext = DbContext;
            landInfo.DictList = dictList;
            landInfo.FileName = fileName;
            landInfo.ExcelName = ExcelName;
            landInfo.TableType = TableType;
            landInfo.InitalizeInnerData();
            bool success = landInfo.ReadTableInformation(isNotLand, IsCheckLandNumberRepeat);
            return success;
        }

        #endregion Methods - ReadInformation

        #region Methods - VerifyInformtaion

        /// <summary>
        /// 校验承包台账调查表信息
        /// </summary>
        public virtual bool VerifyLandTableInformation()
        {
            if (landInfo == null)
            {
                return false;
            }
            var isLandNumberRepeat = CheckLandNumberRepeat();
            bool success = landInfo.CheckImportData(landInfo.LandFamilyCollection);
            if (landInfo.IsContaionTableValue)
            {
                success = landInfo.CheckImportSecondTableData(landInfo.LandFamilyCollection);
            }
            ShowInformation(landInfo.WarnInformation);
            ShowErrowInformation(landInfo.ErrorInformation);
            return success && isLandNumberRepeat;
        }

        /// <summary>
        /// 检查地块编码是否重复，主要和确股地块比较，因为确权地块已被清空
        /// </summary>
        protected bool CheckLandNumberRepeat()
        {
            bool checkResult = true;
            var stocklands = DbContext.CreateContractLandWorkstation().Get(o => o.IsStockLand == true);
            foreach (var stockLand in stocklands)
            {
                foreach (var landFamily in landInfo.LandFamilyCollection)
                {
                    foreach (var land in landFamily.LandCollection)
                    {
                        if (land.LandNumber == stockLand.LandNumber)
                        {
                            checkResult = false;
                            landInfo.ErrorInformation.Add("表中地块编码" + land.LandNumber + "与确股地块编码重复");
                        }
                    }
                }
            }
            return checkResult;
        }

        #endregion Methods - VerifyInformtaion

        #region Methods - ImportEntity

        /// <summary>
        /// 导入实体
        /// </summary>
        /// <returns></returns>
        public virtual bool ImportLandEntity()
        {
            try
            {
                DbContext.BeginTransaction();
                if (landInfo.LandFamilyCollection == null || landInfo.LandFamilyCollection.Count == 0)
                {
                    this.ReportInfomation("无数据可以导入！");
                    return false;
                }

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
                remainVps = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                remainLands = landStation.GetCollection(CurrentZone.FullCode, eLevelOption.Self);
                remainConcords = concordStation.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                remainBooks = bookStation.GetByZoneCode(CurrentZone.FullCode, eSearchOption.Fuzzy);
                foreach (LandFamily landFamily in landInfo.LandFamilyCollection)
                {
                    if (!CheckConcordRegeditBook(landFamily))
                    {
                        return false;
                    }
                    if (!isOk)
                    {
                        return false;
                    }

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
                return true;
            }
            catch (Exception ex)
            {
                DbContext.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportLandEntity(导入地籍调查表失败!)", ex.Message + ex.StackTrace);
                return ReportErrorInfo("导入时发生错误,请检查地籍表格结构或内容是否正确! " + ex.Message);
            }
            finally
            {
                landInfo.Dispose();
            }
        }

        /// <summary>
        /// 检查合同与权证信息
        /// </summary>
        /// <param name="landFamily">人和地集合</param>
        protected bool CheckConcordRegeditBook(LandFamily landFamily)
        {
            if (landFamily.Concord != null && !string.IsNullOrEmpty(landFamily.Concord.ConcordNumber) && concordBusiness.Exists(landFamily.Concord.ConcordNumber))
            {
                ContractConcord cc = concordBusiness.Get(landFamily.Concord.ConcordNumber);
                if (cc == null)
                    return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的合同编号重复！");
                Zone z = VirtualGetZone(cc.ZoneCode);
                if (z == null)
                    return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的合同编号重复！");
                return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的合同编号" + landFamily.Concord.ConcordNumber + "与" + z.FullName + "下户" + cc.ContracterName + "的合同编号一致！");
            }
            if (landFamily.RegeditBook != null && !string.IsNullOrEmpty(landFamily.RegeditBook.RegeditNumber) && contractRegeditBookBusiness.Exists(landFamily.RegeditBook.RegeditNumber))
            {
                ContractConcord cc = concordBusiness.Get(landFamily.RegeditBook.ID);
                if (cc == null)
                    return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的证书编号重复！");
                Zone z = VirtualGetZone(cc.ZoneCode);
                if (z == null)
                    return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的证书编号重复！");
                return ReportErrorInfo("户：" + landFamily.CurrentFamily.Name + "的证书编号" + landFamily.RegeditBook.RegeditNumber + "与 " + z.FullName + "下户" + cc.ContracterName + "的证书编号一致！");
            }
            return true;
        }

        /// <summary>
        /// 导入信息
        /// </summary>
        public virtual void ImportLandFamily(LandFamily landFamily, int familyIndex)
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
                if (!string.IsNullOrEmpty(land.LandNumber) && land.LandNumber.Length >= 5)
                {
                    land.SurveyNumber = land.LandNumber.Substring(land.LandNumber.Length - 5);
                }//InitalizeAgricultureLandShare(land);
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

        #region 导入承包方数据

        /// <summary>
        /// 导入承包方数据
        /// </summary>
        /// <returns></returns>
        public virtual VirtualPerson ImportVirtualPersonInformation(LandFamily landFamily, int familyIndex)
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
                    landFamily.CurrentFamily.ID = vp.ID;
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
                YuLinTu.Library.Log.Log.WriteException(this, "FindVirtualPerson(导入承包方数据失败!)", ex.Message + ex.StackTrace);
                throw new YltException("导入承包方数据失败!");
            }
            return vp;
        }

        /// <summary>
        /// 导入承包方
        /// </summary>
        public virtual VirtualPerson ImportVirtualPerson(LandFamily landFamily, int familyIndex)
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

        #endregion 导入承包方数据

        #region 导入合同与权证信息

        /// <summary>
        /// 导入合同与权证信息
        /// </summary>
        /// <param name="landFamily"></param>
        /// <param name="vp"></param>
        protected void ImportConcordAndRegeditBook(LandFamily landFamily, VirtualPerson vp)
        {
            if (!isOk)
                return;
            if (vp.Name.IndexOf("机动地") >= 0 || vp.Name.IndexOf("集体") >= 0)
            {
                //集体户不签订合同和权证
                return;
            }
            if (landFamily.Concord != null)
            {
                landFamily.Concord.ContracterId = vp.ID;
                landFamily.Concord.ContracterName = vp.Name;
                landFamily.Concord.Founder = "Admin";
                landFamily.Concord.CreationTime = DateTime.Now;
                if (tissue != null)
                {
                    landFamily.Concord.SenderId = tissue.ID;
                    landFamily.Concord.SenderName = tissue.Name;
                }
                //string number = landFamily.CurrentFamily.SenderCode.PadRight(14, '0') + landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0') + (landFamily.Concord.ArableLandType == "110" ? "J" : "Q");
                if (string.IsNullOrEmpty(landFamily.Concord.ConcordNumber))
                {
                    landFamily.Concord.ConcordNumber = "";
                }
                if (string.IsNullOrEmpty(landFamily.Concord.ContractCredentialNumber))
                {
                    landFamily.Concord.ContractCredentialNumber = "";
                }
                concordBusiness.Add(landFamily.Concord);
                foreach (var item in landFamily.LandCollection)
                {
                    item.ConcordId = landFamily.Concord.ID;
                }
            }
            if (landFamily.RegeditBook != null)
            {
                landFamily.RegeditBook.Number = landFamily.Concord.ConcordNumber;
                landFamily.RegeditBook.RegeditNumber = landFamily.Concord.ContractCredentialNumber;
                landFamily.RegeditBook.CreationTime = DateTime.Now;
                landFamily.RegeditBook.Founder = "Admin";
                landFamily.RegeditBook.ID = landFamily.Concord.ID;
                landFamily.RegeditBook.Year = DateTime.Now.Year.ToString();
                landFamily.RegeditBook.ZoneCode = CurrentZone.FullCode;
                contractRegeditBookBusiness.AddRegeditBook(landFamily.RegeditBook);
            }
        }

        #endregion 导入合同与权证信息

        #region 二轮台账

        /// <summary>
        /// 导入二轮承包方
        /// </summary>
        protected void ImportTableInformation(LandFamily landFamily, int familyIndex)
        {
            if (landInfo.IsContaionTableValue)
            {
                InportTableVirtualPerson(landFamily);
            }
            if (landInfo.IsContaionTableLandValue)
            {
                ImportTableLandInformation(landFamily);
            }
        }

        /// <summary>
        /// 导入二轮承包方信息
        /// </summary>
        public virtual void InportTableVirtualPerson(LandFamily landFamily)
        {
            if (landFamily.TablePersons == null)
            {
                return;
            }
            VirtualPerson vp = new VirtualPerson();
            vp.ID = landFamily.CurrentFamily.ID;
            vp.Name = landFamily.TableFamily.Name;
            vp.FamilyNumber = landFamily.TableFamily.FamilyNumber;
            vp.TotalArea = landFamily.TableFamily.TotalArea;
            vp.TotalTableArea = landFamily.TableFamily.TotalTableArea;
            vp.TotalActualArea = landFamily.TableFamily.TotalActualArea;
            vp.TotalAwareArea = landFamily.TableFamily.TotalAwareArea;
            vp.TotalModoArea = landFamily.TableFamily.TotalModoArea;
            //生成承包方中的共有人信息
            List<Person> personList = new List<Person>();
            foreach (Person per in landFamily.TablePersons)
            {
                if (per == null)
                {
                    continue;
                }
                if (per.Name == landFamily.TableFamily.Name)
                {
                    vp.Number = per.ICN;
                }
                personList.Add(per);
                personCount++;
            }
            vp.PersonCount = landFamily.TableFamily.PersonCount != null ? landFamily.TableFamily.PersonCount.ToString() : personList.Count.ToString();
            vp.SharePersonList = personList;
            vp.ZoneCode = CurrentZone.FullCode;
            vp.VirtualType = eVirtualPersonType.Family;
            vp.Address = CurrentZone.FullName;
            vp.CreationTime = DateTime.Now;
            vp.OtherInfomation = landFamily.TableFamily.OtherInfomation;
            if (string.IsNullOrEmpty(vp.SharePerson))
            {
                return;
            }
            if (!landInfo.isContainTableFamilyExpandValue)
            {
                vp.OtherInfomation = "";
            }
            personBusiness.Add(vp);
            familyCount++;
        }

        /// <summary>
        /// 导入二轮承包地块信息
        /// </summary>
        protected void ImportTableLandInformation(LandFamily landFamily)
        {
            if (landFamily.TableLandCollection == null || landFamily.TableLandCollection.Count == 0)
            {
                return;
            }
            foreach (SecondTableLand land in landFamily.TableLandCollection)
            {
                land.OwnerId = landFamily.CurrentFamily.ID;
                landBusiness.AddLand(land);
                landCount++;
            }
        }

        #endregion 二轮台账

        #region 导入地块数据

        /// <summary>
        /// 合并承包地块
        /// </summary>
        public virtual List<ContractLand> CombinationLand(List<ContractLand> lands, VirtualPerson vp)
        {
            if (lands == null || lands.Count < 1)
                return lands;

            //把所有承包地添加到目标数据源中
            for (int i = 0; i < lands.Count; i++)
            {
                if (lands[i] == null)
                    continue;

                lands[i].ZoneCode = CurrentZone.FullCode;
                lands[i].ZoneName = CurrentZone.FullName;

                //设置地块的权属单位
                //如果存在相应的集体经济组织则其权属单位为对应集体经济组织，否则为当前地域（地域是默认的集体经济组织）
                //CollectivityTissue tissue = concordBusiness.GetSenderById(CurrentZone.ID);
                if (sender != null)
                {
                    lands[i].SenderCode = sender.Code;
                    lands[i].SenderName = sender.Name;
                }
                else
                {
                    lands[i].SenderCode = CurrentZone.FullCode;
                    lands[i].SenderName = CurrentZone.FullName;
                }

                lands[i].Founder = "Admin";
                lands[i].CreationTime = DateTime.Now;

                //设置承包地的承包方
                lands[i].OwnerId = vp.ID;
                lands[i].OwnerName = vp.Name;
            }

            return lands;
        }

        /// <summary>
        /// 初始化地块共享
        /// </summary>
        protected void InitalizeAgricultureLandShare(ContractLand land)
        {
            //AgricultureLandExpand expand = land.LandExpand;
            //if (expand == null || string.IsNullOrEmpty(expand.ShareInformation))
            //{
            //    return;
            //}
            //string[] familyString = expand.ShareInformation.Split(new char[] { '|' });
            //if (familyString == null || familyString.Length == 0)
            //{
            //    return;
            //}
            //List<AgricultureSharedLand> lands = new List<AgricultureSharedLand>();
            //for (int i = 0; i < familyString.Length; i++)
            //{
            //    string[] shareString = familyString[i].Split(new char[] { ',' });
            //    if (shareString == null || shareString.Length != 5)
            //    {
            //        continue;
            //    }
            //    double area = 0.0;
            //    AgricultureSharedLand sLand = new AgricultureSharedLand();
            //    sLand.FamilyName = shareString[0];
            //    if (string.IsNullOrEmpty(sLand.FamilyName))
            //    {
            //        continue;
            //    }
            //    LandFamily lf = landInfo.LandFamilyCollection.Find(fam => fam.CurrentFamily.Name == sLand.FamilyName);
            //    sLand.FamilyId = lf != null ? lf.CurrentFamily.ID : Guid.Empty;
            //    double.TryParse(shareString[1], out area);
            //    sLand.TableArea = area;
            //    double.TryParse(shareString[2], out area);
            //    sLand.ActualArea = area;
            //    double.TryParse(shareString[3], out area);
            //    sLand.SharedArea = area;
            //    sLand.Comment = shareString[4];
            //    lands.Add(sLand);
            //}
            //AgricultureShareExpand se = new AgricultureShareExpand();
            //se.SharedLands = lands;
            //land.ExtendC = se.ToString();
            //lands = null;
        }

        /// <summary>
        /// 导入承包地块
        /// </summary>
        public virtual void ImportContractLand(List<ContractLand> lands)
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
                    if (!string.IsNullOrEmpty(temp.ZoneCode)
                       && (temp.ZoneCode == CurrentZone.UpLevelCode || temp.ZoneCode == CurrentZone.FullCode))
                    {
                        landBusiness.Delete(temp.ID);
                    }
                    else
                    {
                        this.ReportErrorInfo("Excel中承包方" + land.OwnerName + "的地块地块编码号:" + ContractLand.GetLandNumber(land.CadastralNumber) + "与" +
                            temp.ZoneName + "下承包方" + temp.OwnerName + " 的地块地块编码重复");
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
                //    if (!string.IsNullOrEmpty(temp.ZoneCode)
                //        && (temp.ZoneCode == CurrentZone.UpLevelCode || temp.ZoneCode == CurrentZone.FullCode))
                //    {
                //        landBusiness.Delete(temp.ID);
                //    }
                //    else
                //    {
                //        this.ReportErrorInfo("Excel中户 " + land.OwnerName + "的地块地块编码号:" + ContractLand.GetLandNumber(land.CadastralNumber) + "与" +
                //            temp.ZoneName + "下 户" + temp.OwnerName + " 的地块地块编码重复");
                //        isOk = false;
                //        return;
                //    }
                //}

                land.Name = land.Name.Replace("\0", "");
                if (land.LandNumber.Length != 19)
                    land.LandNumber = $"{land.ZoneCode}{land.SurveyNumber}";
                if (land.Comment.Contains("自留地"))
                    land.LandCategory = "21";
                landBusiness.AddLand(land);
                landCount++;
            }
        }

        /// <summary>
        /// 检查同村下调查编码
        /// </summary>
        protected void CheckSurveyNumber(ContractLand land)
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
                        this.ReportExcetionInfo("地块编码" + landNumber + "在" + conLand.ZoneName + "下已经存在!");
                    }
                }
            }
        }

        #endregion 导入地块数据

        #endregion Methods - ImportEntity

        #region Methods - Clear

        /// <summary>
        /// 清除地籍数据
        /// </summary>
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

        /// <summary>
        /// 删除地块信息
        /// </summary>
        //protected void ClearLandInformtion(List<VirtualPerson> familys, List<VirtualPerson> unLockFamilys)
        //{
        //    bool contractorClear = SettingDefine.ClearVirtualPersonData;
        //    DeleteAllLandDataByZone(unLockFamilys, contractorClear);
        //    if (familys != null && familys.Count == 0)
        //    {
        //        ClearAllRelationData(contractorClear);
        //    }
        //    else
        //    {
        //        DeleteAllLandDataByZone(unLockFamilys, contractorClear);
        //    }
        //}

        /// <summary>
        /// 过滤承包方
        /// </summary>
        protected List<VirtualPerson> FilterContractor(List<VirtualPerson> familys, List<VirtualPerson> unLockFamilys)
        {
            List<VirtualPerson> personCollection = new List<VirtualPerson>();
            foreach (VirtualPerson person in unLockFamilys)
            {
                VirtualPerson vp = familys.Find(data => data.Name == person.Name && data.Number == person.Number);
                if (vp == null)
                {
                    personCollection.Add(person);
                }
            }
            return personCollection;
        }

        /// <summary>
        /// 清除所有相关数据
        /// </summary>
        protected void ClearAllRelationData(bool vpClear)
        {
            try
            {
                int count = contractRegeditBookBusiness.GetByZoneCode(CurrentZone.FullCode, eSearchOption.Precision).Count;
                if (count == 0)
                {
                    List<ContractConcord> entitys = concordBusiness.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                    foreach (ContractConcord concord in entitys)
                    {
                        if (concord.ID != null)
                        {
                            contractRegeditBookBusiness.Delete(concord.ID);
                        }
                        if (concord.RequireBookId.HasValue)
                        {
                            concordBusiness.DeleteRequireTable(concord.RequireBookId.Value);
                        }
                    }
                    entitys = null;
                }
                concordBusiness.DeleteByZoneCode(CurrentZone.FullCode);
                landBusiness.DeleteLandByZoneCode(CurrentZone.FullCode);//删除所有土地信息
                dotStation.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                coilStation.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                //DataInstance.BuildLandBoundaryAddressDot.DeleteByZoneCode(CurrentZone.FullCode, eSearchType.Precision, eLandPropertyRightType.AgricultureLand);
                //DataInstance.BuildLandBoundaryAddressCoil.DeleteByZoneCode(CurrentZone.FullCode, eSearchType.Precision, eLandPropertyRightType.AgricultureLand);
                contractRegeditBookBusiness.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                concordBusiness.DeleteRequireTableByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                if (vpClear)
                {
                    personBusiness.ClearZoneData(CurrentZone.FullCode);
                }
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearAllRelationData(清除所有相关数据失败)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 删除当前地域下所有未被锁定的数据
        /// </summary>
        //protected bool DeleteAllLandDataByZone(List<VirtualPerson> unLockFamilys, bool vpClear)
        //{
        //    List<ContractLand> landCollection = new List<ContractLand>();
        //    List<ContractLand> lands = landBusiness.GetCollection(CurrentZone.FullCode, eLevelOption.Self);
        //    List<ContractConcord> concords = concordBusiness.GetCollection(CurrentZone.FullCode);
        //    foreach (VirtualPerson person in unLockFamilys)
        //    {
        //        List<ContractConcord> entitys = concords.FindAll(cd => cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == person.ID);
        //        if (entitys != null)
        //        {
        //            try
        //            {
        //                foreach (ContractConcord concord in entitys)
        //                {
        //                    if (concord.ID != null)
        //                    {
        //                        contractRegeditBookBusiness.Delete(concord.ID);
        //                    }
        //                    if (concord.RequireBookId.HasValue)
        //                    {
        //                        concordBusiness.DeleteRequireTable(concord.RequireBookId.Value);
        //                    }
        //                    concordBusiness.Delete(concord.ID);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                YuLinTu.Library.Log.Log.WriteException(this, "DeleteAllLandDataByZone(删除当前地域下所有数据失败)", ex.Message + ex.StackTrace);
        //                throw ex;
        //            }
        //        }
        //        List<ContractLand> landArray = lands.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == person.ID);
        //        foreach (ContractLand land in landArray)
        //        {
        //            landBusiness.Delete(land.ID);
        //            //DataInstance.BuildLandBoundaryAddressCoil.SL_Delete("LandID", land.ID);
        //            //DataInstance.BuildLandBoundaryAddressDot.SL_Delete("LandID", land.ID);
        //            //DataInstance.PropertyLineNeighbor.SL_Delete("LandID", land.ID);
        //            //DataInstance.CadastralInvestigate.SL_Delete("LandID", land.ID);
        //            landCollection.Add(land);
        //        }
        //        if (vpClear)
        //        {
        //            personBusiness.Delete(person.ID);
        //        }
        //        entitys = null;
        //        landArray = null;
        //    }
        //    lands = null;
        //    concords = null;
        //    landCollection = null;
        //    return true;
        //}

        /// <summary>
        /// 删除当前地域下所有未被锁定的数据
        /// </summary>
        /// <param name="vpClear">是否清空承包方数据</param>
        public virtual bool DeleteAllLandDataByZone(bool vpClear)
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

        /// <summary>
        /// 删除承包方所有信息
        /// </summary>
        //protected void DeleteVirtualPerson(ContainerFactory factory, List<VirtualPerson> lockfamilys)
        //{
        //    IVirtualPersonWorkStation<LandVirtualPerson> vpStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
        //    var familys = vpStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
        //    if (familys == null) return;
        //    foreach (VirtualPerson vp in lockfamilys)
        //    {
        //        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
        //    }
        //    IContractLandWorkStation contractLandWorkStation = this.DbContext.CreateContractLandWorkstation();//承包台账地块业务逻辑层
        //    IConcordWorkStation concordStation = this.DbContext.CreateConcordStation();
        //    IContractRegeditBookWorkStation contractRegeditBookStation = this.DbContext.CreateRegeditBookStation();
        //    IBuildLandBoundaryAddressCoilWorkStation jzxStation = this.DbContext.CreateBoundaryAddressCoilWorkStation();
        //    IBuildLandBoundaryAddressDotWorkStation jzdStation = this.DbContext.CreateBoundaryAddressDotWorkStation();
        //    ISecondTableLandWorkStation secondtableLandStation = this.DbContext.CreateSecondTableLandWorkstation();

        //    foreach (VirtualPerson vp in familys)
        //    {
        //        secondtableLandStation.DeleteLandByPersonID(vp.ID);
        //        var lands = contractLandWorkStation.GetCollection(vp.ID);
        //        foreach (var landitem in lands)
        //        {
        //            var jzds = jzdStation.GetByLandID(landitem.ID).Select(d => d.ID).ToList();
        //            var jzxs = jzxStation.GetByLandID(landitem.ID).Select(d => d.ID).ToList();
        //            if (jzds != null && jzds.Count > 0)
        //            {
        //                jzds.ForEach(d => jzdStation.Delete(d));
        //            }
        //            if (jzxs != null && jzxs.Count > 0)
        //            {
        //                jzxs.ForEach(d => jzxStation.Delete(d));
        //            }
        //        }
        //        contractLandWorkStation.DeleteLandByPersonID(vp.ID);

        //        var concords = concordStation.GetContractsByFamilyID(vp.ID);
        //        if (concords != null && concords.Count > 0)
        //        {
        //            foreach (var item in concords)
        //            {
        //                var regbook = contractRegeditBookStation.Get(item.ID);
        //                if (regbook != null)
        //                {
        //                    contractRegeditBookStation.Delete(regbook.ID);
        //                }
        //            }
        //        }
        //        vpStation.Delete(vp.ID);
        //    }
        //    var vpguids = familys.Select(f => f.ID).ToList();
        //    concordStation.DeleteByOwnerIds(vpguids);
        //    familys.Clear();
        //}

        /// <summary>
        /// 注销
        /// </summary>
        public void Disponse()
        {
            DbContext = null;
            CurrentZone = null;
            concrdNumberList = null;
            landInfo.Dispose();
            GC.Collect();
        }

        #endregion Methods - Clear

        #region Methods - Report

        /// <summary>
        /// 报告异常信息
        /// </summary>
        protected bool ReportExcetionInfo(string message)
        {
            this.ReportWarn(message);
            return false;
        }

        /// <summary>
        /// 报告错误信息
        /// </summary>
        protected bool ReportErrorInfo(string message)
        {
            this.ReportError(message);
            return false;
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        protected void ShowErrowInformation(List<string> errorArray)
        {
            if (errorArray == null || errorArray.Count == 0)
            {
                return;
            }
            foreach (var item in errorArray)
            {
                ReportErrorInfo(item);
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        protected void ShowInformation(List<string> inforArray)
        {
            if (inforArray == null || inforArray.Count == 0)
            {
                return;
            }
            foreach (var item in inforArray)
            {
                ReportExcetionInfo(item);
            }
        }

        #endregion Methods - Report

        #region Methods - Helper

        /// <summary>
        /// 设置进度
        /// </summary>
        public void SetProgress()
        {
            int count = 0;
            concrdNumberList = new List<string>();
            if (landInfo.LandFamilyCollection == null)
            {
                return;
            }
            foreach (var item in landInfo.LandFamilyCollection)
            {
                count += 1;
                count += item.Persons.Count;
                count += item.LandCollection.Count;
                if (item.Concord == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(item.Concord.ConcordNumber))
                {
                    concrdNumberList.Add(item.Concord.ConcordNumber);
                }
            }
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        public Zone VirtualGetZone(string zoneCode)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zoneCode;
            arg.Name = Name = ZoneMessage.VIRTUALPERSON_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        #endregion Methods - Helper

        #endregion Methods
    }
}