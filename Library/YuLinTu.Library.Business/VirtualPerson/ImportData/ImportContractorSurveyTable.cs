/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Text.RegularExpressions;
using System.Collections;
using YuLinTu.Library.Office;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入承包方调查表
    /// </summary>
    [Serializable]
    public class ImportContractorSurveyTable : Task
    {
        #region Fields

        private IDbContext dataInstance;//数据库
        private ToolProgress toolProgress;//进度工具
        private List<VirtualPerson> tableFamilys;
        private List<VirtualPerson> familyCollection;
        private int personCount;//所有人数
        private int familyCount;//所有户数
        private bool isSynchronous;//是否同步(导入承包经营权数据,导入其他权数据)
        private FamilyImportDefine familyImportSet;
        private FamilyOtherDefine familyOtherSet;

        #endregion

        #region Propertys

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DataInstance
        {
            get { return dataInstance; }
            set { dataInstance = value; }
        }

        /// <summary>
        /// 表类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> Familys { get { return familyCollection; } }

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        public FamilyImportDefine FamilyImportSet
        {
            get { return familyImportSet; }
            set { familyImportSet = value; }
        }

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet
        {
            get { return familyOtherSet; }
            set { familyOtherSet = value; }
        }

        /// <summary>
        /// 百分比进度
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比进度
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        /// <summary>
        /// 表格的名称
        /// </summary>
        public string TableName { get; set; }

        #endregion

        #region Ctor

        public ImportContractorSurveyTable()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion

        #region Methods - ReadInformation

        /// <summary>
        /// 读取户籍信息
        /// </summary>
        public bool ReadContractorInformation(string fileName, bool isStockRight)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            string message = string.Empty;
            string information = string.Empty;
            using (ReadFamilyInformation familyInfo = new ReadFamilyInformation(fileName))
            {
                familyInfo.TableType = TableType;
                familyInfo.FamilyImportSet = FamilyImportSet;   //导入配置文件
                //familyInfo.OtherDefine = FamilyOtherSet;
                familyInfo.OpenFamilyFile();
                familyInfo.ReadInformation(isStockRight);
                familyCollection = familyInfo.ExcelFamilys;
                tableFamilys = familyInfo.TableFamilys;
                message = familyInfo.ErrorInformation;
                information = familyInfo.PromptInformation;
            }
            if (!string.IsNullOrEmpty(information))
            {
                string[] messages = information.Split('!');
                for (int index = 0; index < messages.Length; index++)
                {
                    if (string.IsNullOrEmpty(messages[index]))
                    {
                        continue;
                    }
                    this.ReportError(messages[index]);
                }
                information = string.Empty;
            }
            if (!string.IsNullOrEmpty(message))
            {
                string[] messages = message.Split('!');
                for (int index = 0; index < messages.Length; index++)
                {
                    if (string.IsNullOrEmpty(messages[index]))
                    {
                        continue;
                    }
                    this.ReportError(messages[index]);
                }
                message = string.Empty;
                return false;
            }
            return true;
        }

        #endregion

        #region Methods - VerifyInformation

        /// <summary>
        /// 校验户籍信息
        /// </summary>
        /// <returns></returns>
        public bool VerifyCensusInfo()
        {
            try
            {
                bool canContinue = VerfiyFamilyInfo();
                if (!canContinue)
                {
                    this.ReportError("数据存在问题,请改正后再继续操作!");
                    this.ReportProgress(100, null);
                }
                return canContinue;
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteError(this, "VerifyCensusInfo(校验户籍信息)", ex.Message + ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// 校验户籍信息
        /// </summary>
        private bool VerfiyFamilyInfo()
        {
            VerifyFamilyInformation verifyFamily = new VerifyFamilyInformation(familyCollection);
            //verifyFamily.FamilyOtherSet = FamilyOtherSet;
            bool isRight = verifyFamily.VerifyFamily();
            ReportInformation(verifyFamily.ErrorInformation, true);
            ReportInformation(verifyFamily.WarnInformation, false);
            verifyFamily.ErrorInformation = string.Empty;
            verifyFamily.WarnInformation = string.Empty;
            return isRight;
        }

        /// <summary>
        /// 报告信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError"></param>
        private void ReportInformation(string message, bool isError)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            string[] messages = message.Split('!');
            for (int index = 0; index < messages.Length; index++)
            {
                if (string.IsNullOrEmpty(messages[index]))
                {
                    continue;
                }
                if (isError)
                    this.ReportError(messages[index]);
                else
                    this.ReportWarn(messages[index]);
            }
        }

        #endregion

        #region Methods

        #region SetValue

        /// <summary>
        /// 设置人口值
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private Person SetPersonValue(Person person)
        {
            if (string.IsNullOrEmpty(person.ICN))
                return person;
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
                }
            }
            return person;
        }

        /// <summary>
        /// 设置承包方与人口列表
        /// </summary>
        public void InitalizeVirtualPersonInformatiion()
        {
            foreach (VirtualPerson vp in familyCollection)
            {
                InitalizeVirtualPerson(vp);
            }
            foreach (VirtualPerson vp in tableFamilys)
            {
                InitalizeVirtualPerson(vp);
            }
        }

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        private void InitalizeVirtualPerson(VirtualPerson landFamily)
        {
            List<Person> plist = landFamily.SharePersonList;
            foreach (Person per in plist)
            {
                if (per.Name == landFamily.Name)
                {
                    landFamily.Number = per.ICN;
                    landFamily.CardType = per.CardType;
                    per.Relationship = string.IsNullOrEmpty(per.Relationship) ? "户主" : per.Relationship;
                    per.ID = landFamily.ID;
                    if (per.Name.Contains("集体"))
                    {
                        per.Relationship = "本人";
                    }
                }
                if (per.CardType == eCredentialsType.IdentifyCard)
                {
                    SetPersonValue(per);
                }
            }
            landFamily.SharePersonList = plist;
            landFamily.VirtualType = eVirtualPersonType.Family;
            landFamily.CreationTime = DateTime.Now;
            landFamily.ZoneCode = CurrentZone.FullCode;
            landFamily.Address = string.IsNullOrEmpty(landFamily.Address) ? CurrentZone.FullName : landFamily.Address;
            landFamily.PersonCount = plist.Count > 0 ? plist.Count.ToString() : "";
            InitalizeFamilyExpandInformation(landFamily, landFamily.FamilyExpand);
            //landFamily = null;
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeFamilyExpandInformation(VirtualPerson family, VirtualPersonExpand expand)
        {
            if (expand == null)
            {
                return;
            }
            VirtualPersonExpand familyExpand = expand;
            familyExpand.ID = family.ID;
            familyExpand.Name = family.Name;
            familyExpand.HouseHolderName = family.Name;
            if (family.Name.Contains("集体"))
            {
                familyExpand.ContractorType = eContractorType.Unit;
            }
            family.FamilyExpand = familyExpand;
        }

        #endregion

        #region ImprotEntityToDB

        /// <summary>
        /// 导入实体
        /// </summary>
        /// <returns></returns>
        public bool ImprotEntitys()
        {
            try
            {
                dataInstance.BeginTransaction();
                ClearData();
                toolProgress.InitializationPercent(familyCollection.Count, Percent, CurrentPercent);
                isSynchronous = AgricultureSetting.SystemVirtualPersonSynchronous;
                ContainerFactory factory = new ContainerFactory(dataInstance);
                IVirtualPersonWorkStation<LandVirtualPerson> landStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
                IVirtualPersonWorkStation<YardVirtualPerson> yardStation = factory.CreateVirtualPersonStation<YardVirtualPerson>();
                IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = factory.CreateVirtualPersonStation<HouseVirtualPerson>();
                IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = factory.CreateVirtualPersonStation<WoodVirtualPerson>();
                IVirtualPersonWorkStation<TableVirtualPerson> tableStation = factory.CreateVirtualPersonStation<TableVirtualPerson>();
                IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = factory.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
                foreach (VirtualPerson vp in familyCollection)
                {
                    if (!isSynchronous)
                    {
                        ImportSingleTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                        toolProgress.DynamicProgress(ZoneDesc + vp.Name);
                    }
                    else
                    {
                        bool result = ImportSingleTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                        if (!result)
                        {
                            continue;
                        }
                        ImportMultiTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                        toolProgress.DynamicProgress(ZoneDesc + vp.Name);
                    }
                }
                foreach (VirtualPerson vp in tableFamilys)
                {
                    tableStation.Add(vp);
                }
                dataInstance.CommitTransaction();
                if (familyCount == familyCollection.Count)
                {
                    this.ReportInfomation(string.Format("{0}表中共有{1}户数据,成功导入{2}户、{3}条共有人记录", TableName,
                        familyCollection.Count, familyCollection.Count, personCount));
                }
                else
                {
                    this.ReportError(string.Format("{0}表中共有{1}户数据,成功导入{2}户、{3}条共有人记录,其中{4}户数据被锁定!", TableName,
                        familyCollection.Count, familyCount, personCount, familyCollection.Count - familyCount));
                }
            }
            catch (Exception ex)
            {
                dataInstance.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteError(this, "ImprotEntitys(导入实体)", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 插入单张表数据
        /// </summary>
        private bool ImportSingleTable(VirtualPerson vp, IVirtualPersonWorkStation<LandVirtualPerson> landStation,
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation,
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation,
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation,
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation)
        {
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    if (!landStation.ExistsLockByZoneCodeAndName(CurrentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = landStation.GetFamilyNumber(vp.FamilyNumber, CurrentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            this.ReportError(string.Format("承包方:{0}的户号{1}已经被{2}使用,该户将不会被导入!", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        landStation.Add(vp);
                    }
                    break;
                case eVirtualType.Yard:
                    if (!yardStation.ExistsLockByZoneCodeAndName(CurrentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = yardStation.GetFamilyNumber(vp.FamilyNumber, CurrentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            this.ReportError(string.Format("承包方:{0}的户号{1}已经被{2}使用", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        yardStation.Add(vp);
                    }
                    break;
                case eVirtualType.House:
                    if (!houseStation.ExistsLockByZoneCodeAndName(CurrentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = houseStation.GetFamilyNumber(vp.FamilyNumber, CurrentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            this.ReportError(string.Format("承包方:{0}的户号{1}已经被{2}使用", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        houseStation.Add(vp);
                    }
                    break;
                case eVirtualType.Wood:
                    if (!woodStation.ExistsLockByZoneCodeAndName(CurrentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = woodStation.GetFamilyNumber(vp.FamilyNumber, CurrentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            this.ReportError(string.Format("承包方:{0}的户号{1}已经被{2}使用", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        woodStation.Add(vp);
                    }
                    break;
                case eVirtualType.CollectiveLand:
                    if (!colleStation.ExistsLockByZoneCodeAndName(CurrentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = colleStation.GetFamilyNumber(vp.FamilyNumber, CurrentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            this.ReportError(string.Format("承包方:{0}的户号{1}已经被{2}使用", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        colleStation.Add(vp);
                    }
                    break;
            }
            familyCount++;
            personCount += vp.SharePersonList.Count;
            return true;
        }

        /// <summary>
        /// 插入多张表数据
        /// </summary>
        private void ImportMultiTable(VirtualPerson vp, IVirtualPersonWorkStation<LandVirtualPerson> landStation,
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation,
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation,
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation,
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation)
        {
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    if (VirtualType == eVirtualType.Land)
                        return;
                    landStation.Add(vp);
                    break;
                case eVirtualType.Yard:
                    if (VirtualType == eVirtualType.Yard)
                        return;
                    yardStation.Add(vp);
                    break;
                case eVirtualType.House:
                    if (VirtualType == eVirtualType.House)
                        return;
                    houseStation.Add(vp);
                    break;
                case eVirtualType.Wood:
                    if (VirtualType == eVirtualType.Wood)
                        return;
                    woodStation.Add(vp);
                    break;
                case eVirtualType.CollectiveLand:
                    if (VirtualType == eVirtualType.CollectiveLand)
                        return;
                    colleStation.Add(vp);
                    break;
                    //case eVirtualType.Other:
                    //    if (VirtualType == eVirtualType.Other)
                    //        return;
                    //    landStation.Add(vp);
                    //    break;
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <returns></returns>
        private bool ClearData()
        {
            ContainerFactory factory = new ContainerFactory(dataInstance);
            IVirtualPersonWorkStation<LandVirtualPerson> landStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation = factory.CreateVirtualPersonStation<YardVirtualPerson>();
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = factory.CreateVirtualPersonStation<HouseVirtualPerson>();
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = factory.CreateVirtualPersonStation<WoodVirtualPerson>();
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = factory.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
            List<VirtualPerson> familys = new List<VirtualPerson>();
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    familys = landStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        //landStation.DeleteByZoneCode(CurrentZone.FullCode);
                        landStation.ClearZoneVirtualPersonALLData(CurrentZone.FullCode);
                    }
                    else
                    {
                        //DeleteVirtualPerson<LandVirtualPerson>(landStation, familys);
                        DeleteVirtualPerson(factory, familys);
                    }
                    break;
                case eVirtualType.Yard:
                    familys = yardStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        yardStation.DeleteByZoneCode(CurrentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<YardVirtualPerson>(yardStation, familys);
                    }
                    break;
                case eVirtualType.House:
                    familys = houseStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        houseStation.DeleteByZoneCode(CurrentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<HouseVirtualPerson>(houseStation, familys);
                    }
                    break;
                case eVirtualType.Wood:
                    familys = woodStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        woodStation.DeleteByZoneCode(CurrentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<WoodVirtualPerson>(woodStation, familys);
                    }
                    break;
            }
            ClearTableVirtualPerson();
            familys = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 清空二轮台账信息
        /// </summary>
        private void ClearTableVirtualPerson()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config\" + "FamilyImportDefine.xml";
            if (!System.IO.File.Exists(filePath))
            {
                return;
            }
            FamilyImportDefine landDefine = ToolSerialization.DeserializeXml(filePath, typeof(FamilyImportDefine)) as FamilyImportDefine;
            if (landDefine != null && (landDefine.ExPackageNameIndex > 0 || landDefine.ExPackageNumberIndex > 0
                || landDefine.IsDeadedIndex > 0 || landDefine.LocalMarriedRetreatLandIndex > 0
                || landDefine.PeasantsRetreatLandIndex > 0 || landDefine.ForeignMarriedRetreatLandIndex > 0))
            {
                IVirtualPersonWorkStation<TableVirtualPerson> tableStation = dataInstance.CreateVirtualPersonStation<TableVirtualPerson>();
                tableStation.DeleteByZoneCode(CurrentZone.FullCode);
            }
        }

        /// <summary>
        /// 删除承包方信息
        /// </summary>
        private void DeleteVirtualPerson<T>(IVirtualPersonWorkStation<T> station, List<VirtualPerson> lockfamilys) where T : VirtualPerson
        {
            List<VirtualPerson> familys = InitalizeDeleteVirtualPerson(lockfamilys);
            foreach (VirtualPerson vp in familys)
            {
                station.Delete(vp.ID);
            }
            familys.Clear();
        }

        /// <summary>
        /// 删除承包方所有信息
        /// </summary>
        private void DeleteVirtualPerson(ContainerFactory factory, List<VirtualPerson> lockfamilys)
        {
            List<VirtualPerson> familys = InitalizeDeleteVirtualPerson(lockfamilys);
            IVirtualPersonWorkStation<LandVirtualPerson> landStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
            IContractLandWorkStation contractLandWorkStation = dataInstance.CreateContractLandWorkstation();//承包台账地块业务逻辑层         
            IConcordWorkStation concordStation = dataInstance.CreateConcordStation();
            IContractRegeditBookWorkStation contractRegeditBookStation = DataInstance.CreateRegeditBookStation();
            IBuildLandBoundaryAddressCoilWorkStation jzxStation = DataInstance.CreateBoundaryAddressCoilWorkStation();
            IBuildLandBoundaryAddressDotWorkStation jzdStation = DataInstance.CreateBoundaryAddressDotWorkStation();
            ISecondTableLandWorkStation secondtableLandStation = DataInstance.CreateSecondTableLandWorkstation();

            foreach (VirtualPerson vp in familys)
            {
                secondtableLandStation.DeleteLandByPersonID(vp.ID);
                var lands = contractLandWorkStation.GetCollection(vp.ID);
                foreach (var landitem in lands)
                {
                    var jzds = jzdStation.GetByLandID(landitem.ID).Select(d => d.ID).ToList();
                    var jzxs = jzxStation.GetByLandID(landitem.ID).Select(d => d.ID).ToList();
                    if (jzds != null && jzds.Count > 0)
                    {
                        jzds.ForEach(d => jzdStation.Delete(d));
                    }
                    if (jzxs != null && jzxs.Count > 0)
                    {
                        jzxs.ForEach(d => jzxStation.Delete(d));
                    }
                }
                contractLandWorkStation.DeleteLandByPersonID(vp.ID);

                var concords = concordStation.GetContractsByFamilyID(vp.ID);
                if (concords != null && concords.Count > 0)
                {
                    foreach (var item in concords)
                    {
                        var regbook = contractRegeditBookStation.Get(item.ID);
                        if (regbook != null)
                        {
                            contractRegeditBookStation.Delete(regbook.ID);
                        }
                    }
                }
                landStation.Delete(vp.ID);
            }
            var vpguids = familys.Select(f => f.ID).ToList();
            concordStation.DeleteByOwnerIds(vpguids);
            familys.Clear();
        }

        /// <summary>
        /// 初始化删除信息
        /// </summary>
        /// <param name="lockfamilys"></param>
        /// <returns></returns>
        private List<VirtualPerson> InitalizeDeleteVirtualPerson(List<VirtualPerson> lockfamilys)
        {
            IVirtualPersonWorkStation<LandVirtualPerson> landStation = dataInstance.CreateVirtualPersonStation<LandVirtualPerson>();
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation = dataInstance.CreateVirtualPersonStation<YardVirtualPerson>();
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = dataInstance.CreateVirtualPersonStation<HouseVirtualPerson>();
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = dataInstance.CreateVirtualPersonStation<WoodVirtualPerson>();
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = dataInstance.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
            List<VirtualPerson> familys = new List<VirtualPerson>();
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    familys = landStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.Yard:
                    familys = yardStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.House:
                    familys = houseStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.Wood:
                    familys = woodStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
            }
            return familys;
        }

        #endregion

        #region AddData

        #endregion

        #region Disponse

        /// <summary>
        /// 注销
        /// </summary>
        public void Disponse()
        {
            tableFamilys = null;
            familyCollection = null;
            personCount = 0;
            familyCount = 0;
            DataInstance = null;
            CurrentZone = null;
            GC.Collect();
        }

        #endregion

        #endregion
    }
}
