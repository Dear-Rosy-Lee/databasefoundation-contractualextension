/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Web;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.PropertyRight.ContractLand;
using YuLinTu.Data;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入承包经营权压缩包
    /// </summary>
    partial class ArcLandImportProgress
    {
        #region 农村土地承包经营权

        //private ArrayList SenderList;//发包方集合
        private ServiceSetDefine ServiceSetDefine = ServiceSetDefine.GetIntence();

        /// <summary>
        /// 导入农用地数据
        /// </summary>
        /// <returns></returns>
        private bool ImportAgricultureLandData(ArcLandImporArgument arg)
        {
            bool success = false;
            this.ReportProgress(1, "正在获取数据...");
            try
            {
                database.BeginTransaction();
                if (arg.OpratorName == "DownLoad")
                {
                    success = ImportAgriculturePackageCollection(arg);
                }
                else
                {
                    success = ImportAgriExLandPackage(arg);
                }
                database.CommitTransaction();
                this.ReportProgress(100);
            }
            catch (Exception ex)
            {
                database.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportAgricultureLandData(导入农用地数据失败!)", ex.Message + ex.StackTrace);
                this.ReportError("导入农用地数据失败!");
            }
            return success;
        }

        /// <summary>
        /// 导入承包台账压缩包
        /// </summary>
        /// <param name="metadata"></param>
        private bool ImportAgriExLandPackage(ArcLandImporArgument arg)
        {
            string xmlFileName = ExtractFile(arg.FileName);
            if (string.IsNullOrEmpty(xmlFileName))
            {
                this.ReportAlert(eMessageGrade.Warn, null, "压缩包中无文件信息!");
                return false;
            }
            bool success = false;
            try
            {             
                List<EX_CBJYQ_DJB> bookCollection = ToolSerialization.DeserializeBinary(xmlFileName, typeof(List<EX_CBJYQ_DJB>)) as List<EX_CBJYQ_DJB>;
                if ((bookCollection == null || bookCollection.Count == 0))
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, "压缩包中无地块信息!");
                    return false;
                }
                DeleteExtractFile(xmlFileName);

                ArrayList zoneList = InitalizeBookZone(bookCollection);
                if (zoneList == null || zoneList.Count == 0)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, "数据中无行政区域信息!");
                    return false;
                }
                bool isValide = zoneList.IndexOf(currentZone.FullCode) >= 0;
                if (!isValide)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, "数据包中行政区域与当前选择行政区域不一致!");
                    return false;
                }

                ClearAgriLandRelationData(arg.CurrentZone);

                success = ImportAgriculturePackage(bookCollection);
                GC.Collect();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, "选择压缩包不是系统支持数据压缩包格式,请确认选择压缩包是否正确!");
            }
            finally
            {
                DeleteExtractFile(xmlFileName);
            }
            return success;
        }

        /// <summary>
        /// 导入下载数据
        /// </summary>
        /// <returns></returns>
        private bool ImportAgriculturePackageCollection(ArcLandImporArgument arg)
        {
            try
            {

                bool success = false;
                bool useStandCode = UseStandCode();
                var targetSpatialReference = database.CreateSchema().GetElementSpatialReference(
                 ObjectContext.Create(typeof(Zone)).Schema,
                 ObjectContext.Create(typeof(Zone)).TableName);
                AgriLandEntity landEntity = new AgriLandEntity();
                AgricultureBookMapping landMapping = new AgricultureBookMapping();
                landMapping.DataInstance = database;
                landMapping.Srid = targetSpatialReference.WKID;
                landMapping.IsBusiness = true;
                YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient landService = ServiceHelper.InitazlieServerData(arg.UserName, arg.SessionCode, arg.UseSafeTrans, ServiceSetDefine.BusinessDataAddress);
                List<Zone> zoneCollection = database.CreateZoneWorkStation().GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                IVirtualPersonWorkStation<LandVirtualPerson> personStation = database.CreateVirtualPersonStation<LandVirtualPerson>();
                double percent = 99 / (double)zoneCollection.Count;
                int index = 0;
                foreach (Zone zone in zoneCollection)
                {
                    if (zone.FullCode == null)
                        continue;
                    var searchCode = zone.FullCode;
                    if (!useStandCode && searchCode.Length == 14)
                        searchCode = searchCode.Substring(0, 12) + "00" + searchCode.Substring(12);
                    string information = "下载" + zone.Name + "数据...";
                    this.ReportProgress((int)(index * percent), information);
                    landEntity.Disponse();
                    var data = landService.GetZone(searchCode);
                    if (data == null)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, string.Format("服务器上不存在{0}地域数据!", zone.FullName));
                        continue;
                    }
                    string zoneCode = zone.FullCode.Length == 16 ? (zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14, 2)) : zone.FullCode;
                    //int count = personStation.Count(zoneCode);
                    int count = personStation.Count(z => z.ZoneCode == zoneCode);
                    if (count == 0)
                    {
                        landEntity.InitalizeDataList();
                    }
                    else
                    {
                        landEntity.Tissues = database.CreateSenderWorkStation().GetTissues(zoneCode);
                        landEntity.Contractors = personStation.GetByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
                        landEntity.Lands = database.CreateContractLandWorkstation().GetCollection(zoneCode);
                        landEntity.Concords = database.CreateConcordStation().GetByZoneCode(zoneCode);
                        landEntity.DotCollection = database.CreateBoundaryAddressDotWorkStation().GetByZoneCode(zoneCode, eSearchOption.Precision, ((int)LanderType).ToString());
                        landEntity.LineCollection = database.CreateBoundaryAddressCoilWorkStation().GetByZoneCode(zoneCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                        landEntity.Books = database.CreateRegeditBookStation().GetByZoneCode(zoneCode, eSearchOption.Precision);
                        landEntity.Tables = database.CreateRequireTableWorkStation().GetByZoneCode(zoneCode, eSearchOption.Precision);
                    }
                    landMapping.LandEntity = landEntity;
                    // Transaction Error
                    try
                    {
                        landMapping.DataInstance.BeginTransaction();
                        AgricultureLandDataProgress(zone, landMapping, landService,
                            personStation, searchCode, percent, index);
                        landMapping.DataInstance.CommitTransaction();
                        success = true;
                    }
                    catch (Exception dsa)
                    {
                        var error = dsa.ToString();
                        landMapping.DataInstance.RollbackTransaction();
                        continue;
                    }
                    index++;
                }
                zoneCollection = null;
                landEntity.Disponse();
                landService = null;
                landMapping = null;
                GC.Collect();
                return success;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, "下载数据时出错:" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 使用标准编码
        /// </summary>
        private bool UseStandCode()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            var config = (section.Settings);
            var zoneDefine = config.Clone() as ZoneDefine;
            return zoneDefine == null ? true : zoneDefine.UseStandCode;
        }

        /// <summary>
        /// 农村土地承包经营权数据处理
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="landService"></param>
        private void AgricultureLandDataProgress(Zone zone, AgricultureBookMapping landMapping, YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient landService,
            IVirtualPersonWorkStation<LandVirtualPerson> personStation, string searchCode, double percent, int cIndex)
        {
            landMapping.ImportData = personStation.Count(p => p.ZoneCode == zone.FullCode) == 0;
            List<EX_CBJYQ_DJB> bookCollection = null;
            if (DownloadSettingDefine.DownloadInvestigationData)
            {
                bookCollection = landService.GetRegisterAndInvestExchangeData(searchCode)?.DJBLB?.ToList();
            }
            else if(DownloadSettingDefine.DownloadRegistrationData)
            {
                bookCollection = landService.GetRegisterDetails(searchCode)?.ToList();
            }
           
            if (bookCollection == null || bookCollection.Count == 0)
            {
                var dataName = DownloadSettingDefine.DownloadInvestigationData ? "调查数据" : "登记数据";
                this.ReportAlert(eMessageGrade.Warn, null, 
                    $"{zone.FullName}下未获取到{dataName},请确认本地是否有对应地域节点或者服务器上是否存在数据!");              
                return;
            }
            this.ReportAlert(eMessageGrade.Infomation, null, "开始添加" + zone.FullName + "数据...");
            double showPersent = percent / (double)bookCollection.Count;         
            int curIndex = 0;
            ExhangeCount entry = new ExhangeCount();
            entry.FamilyCount = bookCollection.Count;
            entry.PersonCount = bookCollection.Sum(bk => bk.DJGYR != null ? bk.DJGYR.Count : 0);
            entry.LandCount = bookCollection.Sum(bk => bk.DJCBD != null ? bk.DJCBD.Count : 0);
            foreach (EX_CBJYQ_DJB book in bookCollection)
            {
                if (book.SZDY.Length == 16)
                    book.SZDY = book.SZDY.Substring(0, 12) + book.SZDY.Substring(14, 2);
                landMapping.InitalizeLocalData(book);
                ReportImportInformation(book);
                curIndex++;
                var description = string.Format("{0}({1}/{2})", zone.Name, curIndex, bookCollection.Count);
                this.ReportProgress((int)(percent * cIndex + curIndex * showPersent), description);
            }
            string record = string.Format("{0}共添加总户数{1}、总人数{2}、总地块数{3},全部数据添加成功", zone.FullName, entry.FamilyCount, entry.PersonCount, entry.LandCount);
            this.ReportAlert(eMessageGrade.Infomation, null, record);
            this.ReportAlert(eMessageGrade.Infomation, null, "添加" + zone.FullName + "数据完成");
            //this.ReportProgress(100, zone.FullName + "下载完成");
            bookCollection.Clear();
            landService = null;
            entry.Clear();
        }

        /// <summary>
        /// 报告信息
        /// </summary>
        /// <param name="books"></param>
        private void ReportImportInformation(EX_CBJYQ_DJB djb)
        {
            if (djb.DJCBD == null)
            {
                return;
            }
            foreach (EX_CBJYQ_CBD cbd in djb.DJCBD)
            {
                if (cbd.Shape == null)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, string.Format("地块编码为:{0}的地块无空间信息!", cbd.DKBM));
                }
            }
        }

        /// <summary>
        /// 导出下载数据
        /// </summary>
        /// <returns></returns>
        private bool ImportAgriculturePackage(List<EX_CBJYQ_DJB> bookCollection)
        {
            if (bookCollection == null || bookCollection.Count == 0)
            {
                return false;
            }
            bookCollection = AgricultureBookFilter(bookCollection);
            ArrayList zoneList = InitalizeBookZone(bookCollection);
            if (zoneList == null || zoneList.Count == 0)
            {
                this.ReportAlert(eMessageGrade.Infomation, null, "数据中无行政区域信息!");
                return false;
            }
            bool isValide = zoneList.IndexOf(currentZone.FullCode) >= 0;
            if (!isValide)
            {
                this.ReportAlert(eMessageGrade.Warn, null, "数据包中行政区域与当前选择行政区域不一致!");
                return false;
            }
            bool success = false;
            try
            {
                var zoneStation = database.CreateZoneWorkStation();
                var personStation = database.CreateVirtualPersonStation<LandVirtualPerson>();
                AgriLandEntity landEntity = new AgriLandEntity() { CurrentZone = currentZone };
                int count = personStation.Count(t => t.ZoneCode == currentZone.FullCode);
                if (count == 0)
                {
                    landEntity.InitalizeDataList();
                }
                else
                {
                    landEntity.Tissues = database.CreateSenderWorkStation().GetTissues(currentZone.FullCode);
                    landEntity.Contractors = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
                    landEntity.Lands = database.CreateContractLandWorkstation().GetCollection(currentZone.FullCode);
                    landEntity.Concords = database.CreateConcordStation().GetByZoneCode(currentZone.FullCode);
                    landEntity.DotCollection = database.CreateBoundaryAddressDotWorkStation().GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, ((int)LanderType).ToString());
                    landEntity.LineCollection = database.CreateBoundaryAddressCoilWorkStation().GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                    landEntity.Books = database.CreateRegeditBookStation().GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                    landEntity.Tables = database.CreateRequireTableWorkStation().GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                }
                var targetSpatialReference = database.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Zone)).Schema,
                    ObjectContext.Create(typeof(Zone)).TableName);
                AgricultureBookMapping landMapping = new AgricultureBookMapping();
                landMapping.DataInstance = database;
                landMapping.LandEntity = landEntity;
                landMapping.Srid = targetSpatialReference.WKID;
                landMapping.ImportData = !personStation.Any(t => t.ZoneCode == currentZone.FullCode);
                if (bookCollection == null || bookCollection.Count == 0)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, "选择的压缩包中没有数据,请确认选择数据是否正确!");
                    return success;
                }
                this.ReportAlert(eMessageGrade.Infomation, null, "开始导入" + currentZone.FullName + "数据...");
                double percent = 98 / (double)bookCollection.Count;
                int curIndex = 1;
                foreach (EX_CBJYQ_DJB book in bookCollection)
                {                    
                    this.ReportProgress((int)(curIndex * percent), string.Format("({0}/{1}){2}", curIndex, bookCollection.Count, currentZone.Name + book.CBFMC));
                    if (book.SZDY.Length == 16)
                    {
                        book.SZDY = book.SZDY.Substring(0, 12) + book.SZDY.Substring(14, 2);
                       
                        book.FBF.SZDY = book.SZDY;
                    }
                    try
                    {
                        landMapping.InitalizeLocalData(book);
                        ReportImportInformation(book);
                    }
                    catch (Exception ex)
                    {
                        Log.Log.WriteError(this, "ImportAgriculturePackage", ex.Message + ex.StackTrace);
                        this.ReportAlert(eMessageGrade.Error, null, string.Format("导入{0}的相关数据发生错误!", book.CBFMC));
                    }

                    curIndex++;
                }
                int familyCount = bookCollection != null ? bookCollection.Count : 0;
                int personCount = bookCollection.Sum(bk => bk.DJGYR != null ? bk.DJGYR.Count : 0);
                int landCount = bookCollection.Sum(bk => bk.DJCBD != null ? bk.DJCBD.Count : 0);
                if (familyCount > 0)
                {
                    string record = string.Format("共处理总户数{0}、总人数{1}、总地块数{2},全部数据处理成功", familyCount, personCount, landCount);
                    this.ReportAlert(eMessageGrade.Infomation, null, record);
                    this.ReportAlert(eMessageGrade.Infomation, null, "导入" + currentZone.FullName + "数据完成...");
                }
                else
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, "添加数据失败,请联系技术支持人员!");
                }
                bookCollection.Clear();
                landEntity.Disponse();
                GC.Collect();
                success = true;
                return success;
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ImportAgriculturePackage(导入数据失败!)", ex.Message + ex.StackTrace);
                this.ReportAlert(eMessageGrade.Error, null, "导入数据时出错:" + ex.ToString());
            }
            return success;
        }

        /// <summary>
        /// 农村土地承包经营权数据过滤
        /// </summary>
        /// <param name="bookCollection"></param>
        /// <returns></returns>
        private List<EX_CBJYQ_DJB> AgricultureBookFilter(List<EX_CBJYQ_DJB> bookCollection)
        {
            if (bookCollection == null || bookCollection.Count == 0)
            {
                return bookCollection;
            }
            List<EX_CBJYQ_DJB> survey = bookCollection.FindAll(bk => string.IsNullOrEmpty(bk.HTBH));
            List<EX_CBJYQ_DJB> register = bookCollection.FindAll(bk => !string.IsNullOrEmpty(bk.HTBH));
            List<EX_CBJYQ_DJB> books = new List<EX_CBJYQ_DJB>();
            if (register == null || register.Count == 0)
            {
                books = survey.Clone() as List<EX_CBJYQ_DJB>;
                survey = null;
                bookCollection = null;
                return books;
            }
            foreach (EX_CBJYQ_DJB book in survey)
            {
                var family = register.Find(bk => bk.CBFMC == book.CBFMC && bk.CBFZJHM == book.CBFZJHM);
                if (family == null)
                {
                    books.Add(book);
                    continue;
                }
                family.DJCBD = book.DJCBD;
                family.DJGYR = book.DJGYR;
                books.Add(family);
            }
            survey = null;
            register = null;
            bookCollection = null;
            return books;
        }        

        #region Methods-Clear

        /// <summary>
        /// 清除所有关联数据
        /// </summary>
        /// <param name="landContractor"></param>
        private void ClearAgriLandRelationData(Zone zone)
        {
            if (zone == null)
            {
                return;
            }
            ClearArcLandRelationData(zone.FullCode);
            GC.Collect();
        }

        /// <summary>
        /// 删除指定地域下的关联数据
        /// </summary>
        /// <param name="zone"></param>
        private void ClearArcLandRelationData(string zoneCode)
        {
            ClearContractConcord(zoneCode);
            var concordStation = database.CreateConcordStation();
            concordStation.DeleteByZoneCode(zoneCode);
            var landStation = database.CreateContractLandWorkstation();
            int delLand = landStation.DeleteByZoneCode(zoneCode);//删除所有土地属性信息
            var personStation = database.CreateVirtualPersonStation<LandVirtualPerson>();
            int delPerson = personStation.DeleteByZoneCode(zoneCode);
            var requireTableStation = database.CreateRequireTableWorkStation();
            requireTableStation.DeleteByZoneCode(zoneCode, eSearchOption.Precision);
        }

        /// <summary>
        /// 删除承包信息
        /// </summary>
        /// <param name="zone"></param>
        private void ClearContractConcord(string zoneCode)
        {
            var concordStation = database.CreateConcordStation();
            List<ContractConcord> entitys = concordStation.GetContractsByZoneCode(zoneCode);
            if (entitys == null)
            {
                return;
            }
            var bookStation = database.CreateRegeditBookStation();
            foreach (ContractConcord concord in entitys)
            {
                if (concord.ID != null)
                {
                    bookStation.Delete(concord.ID);
                }
                if (concord.RequireBookId.HasValue)
                {
                    bookStation.Delete(concord.RequireBookId.Value);
                }
            }
            entitys.Clear();
        }

        #endregion

        #region Methods-Import

        /// <summary>
        /// 初始化登记薄地域
        /// </summary>
        /// <param name="bookCollection"></param>
        /// <returns></returns>
        private ArrayList InitalizeBookZone(List<EX_CBJYQ_DJB> bookCollection)
        {
            ArrayList list = new ArrayList();
            foreach (EX_CBJYQ_DJB book in bookCollection)
            {
                string code = book.SZDY.Length == 16 ? (book.SZDY.Substring(0, 12) + book.SZDY.Substring(14, 2)) : book.SZDY;
                if (list.Contains(code))
                {
                    continue;
                }
                list.Add(code);
            }
            return list;
        }

        /// <summary>
        /// 获取要导入的地域集合
        /// </summary>
        /// <param name="landCollection"></param>
        /// <returns></returns>
        //private YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection GetAgriLandZoneCollection(YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection landCollection)
        //{
        //    YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection zoneCollection = new YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection();
        //    if (landCollection == null || landCollection.Count == 0)
        //    {
        //        return zoneCollection;
        //    }
        //    List<string> zones = new List<string>();
        //    foreach (YuLinTu.Business.ContractLand.Exchange2.ExLandContractor land in landCollection)
        //    {
        //        if (land.Zone == null)
        //        {
        //            continue;
        //        }
        //        if (zones.Contains(land.Zone.FullCode))
        //        {
        //            continue;
        //        }
        //        zones.Add(land.Zone.FullCode);
        //        zoneCollection.Add(land.Zone);
        //    }
        //    return zoneCollection;
        //}

        ///// <summary>
        ///// 导入承包方
        ///// </summary>
        ///// <param name="exContractor"></param>
        //private void ImportAgriContractor(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        //{
        //    if (exLandContractor == null)
        //    {
        //        return;
        //    }
        //    var personStation = database.CreateVirtualPersonStation<LandVirtualPerson>();
        //    VirtualPerson virtualPerson = AgriContractorMapping(exLandContractor);
        //    virtualPerson.ID = virtualPerson.ID == Guid.Empty ? exLandContractor.Contractor.ID : virtualPerson.ID;
        //    if (personStation.Get(virtualPerson.ID) == null)
        //    {
        //        personStation.Add(virtualPerson);
        //    }
        //    virtualPerson = null;
        //}

        ///// <summary>
        ///// 承包方映射
        ///// </summary>
        ///// <param name="exContractor"></param>
        ///// <returns></returns>
        //private VirtualPerson AgriContractorMapping(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        //{
        //    YuLinTu.Business.ContractLand.Exchange2.ExContractor exContractor = exLandContractor.Contractor;
        //    if (exContractor == null)
        //    {
        //        return null;
        //    }
        //    VirtualPerson person = landEntity.Contractors.Find(vp => vp.ID == exContractor.ID);
        //    if (person == null)
        //    {
        //        person = landEntity.Contractors.Find(vp => vp.Name == exContractor.Name);
        //    }
        //    if (person == null)
        //    {
        //        person = new VirtualPerson();
        //        person.ID = Guid.Empty;
        //    }
        //    person.PostalNumber = string.IsNullOrEmpty(exContractor.PostNumber) ? person.PostalNumber : exContractor.PostNumber;
        //    person.FamilyNumber = string.IsNullOrEmpty(exContractor.FamilyNumber) ? person.FamilyNumber : InitalizeFamilyNumber(exContractor.FamilyNumber);
        //    person.Comment = string.IsNullOrEmpty(exContractor.Comment) ? person.Comment : exContractor.Comment;
        //    person.CreationTime = exContractor.CreationTime == null ? person.CreationTime : exContractor.CreationTime;
        //    person.Founder = string.IsNullOrEmpty(exContractor.Founder) ? person.Founder : exContractor.Founder;
        //    person.ModifiedTime = exContractor.ModifiedTime == null ? DateTime.Now : exContractor.ModifiedTime;
        //    person.Modifier = string.IsNullOrEmpty(exContractor.Modifier) ? person.Modifier : exContractor.Modifier;
        //    person.Name = string.IsNullOrEmpty(exContractor.Name) ? person.Name : exContractor.Name;
        //    person.Number = string.IsNullOrEmpty(exContractor.Number) ? person.Number : exContractor.Number;
        //    person.Telephone = string.IsNullOrEmpty(exContractor.Telephone) ? person.Telephone : exContractor.Telephone;
        //    person.VirtualType = eVirtualPersonType.Family;
        //    person.ZoneCode = exLandContractor.Zone != null ? exLandContractor.Zone.FullCode : currentZone.FullCode;
        //    person.Address = string.IsNullOrEmpty(exContractor.LandLocated) ? person.Address : exContractor.LandLocated;
        //    person.SharePerson = GetAgriSharePersons(exLandContractor);
        //    return person;
        //}

        /// <summary>
        /// 初始化户号
        /// </summary>
        /// <param name="familyNumber"></param>
        /// <returns></returns>
        private string InitalizeFamilyNumber(string familyNumber)
        {
            if (string.IsNullOrEmpty(familyNumber))
            {
                return "";
            }
            string famNumber = familyNumber;
            if (familyNumber.Length > 14)
            {
                famNumber = familyNumber.Substring(14);
            }
            int number = 0;
            Int32.TryParse(famNumber, out number);
            return number.ToString();
        }

        /// <summary>
        /// 获取共有人信息
        /// </summary>
        /// <param name="espc"></param>
        /// <returns></returns>
        private string GetAgriSharePersons(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExSharePersonCollection espc = exLandContractor.SharePersons;
            if (espc == null || espc.Count == 0)
            {
                return string.Empty;
            }
            List<Person> sharePersons = new List<Person>();
            foreach (YuLinTu.Business.ContractLand.Exchange2.ExSharePerson exSharePerson in espc)
            {
                if (exSharePerson == null)
                {
                    continue;
                }
                exSharePerson.ContractorId = exLandContractor.Contractor.ID;
                Person person = AgriSharePersonMapping(exSharePerson);
                person.ZoneCode = exLandContractor.Zone != null ? exLandContractor.Zone.FullCode : string.Empty;
                sharePersons.Add(person);
            }
            if (sharePersons.Count == 0)
            {
                return string.Empty;
            }
            return ToolSerialize.SerializeXmlString<List<Person>>(sharePersons); ;
        }

        /// <summary>
        /// 共有人映射
        /// </summary>
        /// <param name="exSharePerson"></param>
        /// <returns></returns>
        private Person AgriSharePersonMapping(YuLinTu.Business.ContractLand.Exchange2.ExSharePerson exSharePerson)
        {
            Person person = new Person();
            person.ID = exSharePerson.ID;
            person.Name = exSharePerson.Name;
            person.Gender = (eGender)Enum.Parse(typeof(eGender), exSharePerson.Sex.ToString());
            person.Comment = exSharePerson.Comment;
            person.FamilyID = exSharePerson.ContractorId;
            person.CreateTime = exSharePerson.CreationTime;
            person.ICN = exSharePerson.CredentialNumber;
            person.CreateUser = exSharePerson.Founder;
            person.LastModifyTime = exSharePerson.ModifiedTime;
            person.LastModifyUser = exSharePerson.Modifier;
            person.Name = exSharePerson.Name;
            try
            {
                if (exSharePerson.Nationality != null)
                {
                    person.Nation = (eNation)EnumNameAttribute.GetValue(typeof(eNation), exSharePerson.Nationality);
                }
            }
            catch { person.Nation = eNation.UnKnown; }
            person.Relationship = exSharePerson.Relation;
            person.Birthday = exSharePerson.BirthDate;
            return person;
        }

        /// <summary>
        /// 导入承包合同
        /// </summary>
        /// <param name="exContractConcord"></param>
        //private void ImportAgriContractConcord(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    if (landContractor == null)
        //    {
        //        return;
        //    }
        //    YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord = landContractor.ContractConcord;
        //    if (exContractConcord == null || string.IsNullOrEmpty(exContractConcord.ConcordNumber))
        //    {
        //        return;
        //    }
        //    var concordStation = database.CreateConcordStation();
        //    ContractConcord concord = AgriContractConcordMapping(exContractConcord);
        //    concord.ZoneCode = landContractor.Zone != null ? landContractor.Zone.FullCode : string.Empty;
        //    concord.ID = concord.ID != Guid.Empty ? concord.ID : exContractConcord.ID;
        //    concordStation.Add(concord);
        //    concord = null;
        //}

        /// <summary>
        /// 承包合同映射
        /// </summary>
        /// <param name="exContractConcord"></param>
        /// <returns></returns>
        //private ContractConcord AgriContractConcordMapping(YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord)
        //{
        //    ContractConcord concord = landEntity.Concords.Find(cd => cd.ID == exContractConcord.ID);
        //    if (concord == null)
        //    {
        //        concord = landEntity.Concords.Find(cd => cd.ConcordNumber == exContractConcord.ConcordNumber);
        //    }
        //    if (concord == null)
        //    {
        //        concord = new ContractConcord();
        //        concord.ID = Guid.Empty;
        //    }
        //    concord.ConcordNumber = string.IsNullOrEmpty(exContractConcord.ConcordNumber) ? exContractConcord.AgriConcordNumber : exContractConcord.ConcordNumber;
        //    concord.AgentCrdentialType = exContractConcord.AgentCrdentialType <= 0 ? concord.AgentCrdentialType : ((int)Enum.Parse(typeof(eCredentialsType), exContractConcord.AgentCrdentialType.ToString())).ToString();
        //    concord.AgentName = string.IsNullOrEmpty(exContractConcord.AgentName) ? concord.AgentName : exContractConcord.AgentName;
        //    concord.AgentTelphone = string.IsNullOrEmpty(exContractConcord.AgentTelphone) ? concord.AgentTelphone : exContractConcord.AgentTelphone;
        //    concord.ArableLandEndTime = exContractConcord.ArableLandEndTime == null ? concord.ArableLandEndTime : exContractConcord.ArableLandEndTime;
        //    concord.ArableLandStartTime = exContractConcord.ArableLandStartTime == null ? concord.ArableLandStartTime : exContractConcord.ArableLandStartTime;
        //    concord.ArableLandType = exContractConcord.ArableLandType <= 0 ? concord.ArableLandType : ((int)Enum.Parse(typeof(eConstructMode), exContractConcord.ArableLandType.ToString())).ToString();
        //    concord.BadlandEndTime = exContractConcord.BadlandEndTime == null ? concord.BadlandEndTime : exContractConcord.BadlandEndTime;
        //    concord.BadlandPurpose = exContractConcord.BadlandPurpose <= 0 ? concord.BadlandPurpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractConcord.BadlandPurpose.ToString())).ToString();
        //    concord.BadlandStartTime = exContractConcord.BadlandStartTime == null ? concord.BadlandStartTime : exContractConcord.BadlandStartTime;
        //    concord.BadlandType = exContractConcord.BadlandType <= 0 ? concord.BadlandType : ((int)Enum.Parse(typeof(eConstructMode), exContractConcord.BadlandType.ToString())).ToString();
        //    concord.CheckAgencyDate = exContractConcord.CheckAgencyDate == null ? concord.CheckAgencyDate : exContractConcord.CheckAgencyDate;
        //    concord.Comment = string.IsNullOrEmpty(exContractConcord.Comment) ? concord.Comment : exContractConcord.Comment;
        //    concord.ContracerType = exContractConcord.ContracerType <= 0 ? concord.ContracerType : ((int)Enum.Parse(typeof(eContractorType), exContractConcord.ContracerType.ToString())).ToString();
        //    concord.ContractCredentialNumber = string.IsNullOrEmpty(exContractConcord.ContractCredentialNumber) ? concord.ContractCredentialNumber : exContractConcord.ContractCredentialNumber;
        //    concord.ContractDate = exContractConcord.ContractDate == null ? concord.ContractDate : exContractConcord.ContractDate;
        //    if (exContractConcord.ContractorId != null)
        //    {
        //        concord.ContracterId = exContractConcord.ContractorId;
        //    }
        //    concord.ContracterIdentifyNumber = string.IsNullOrEmpty(exContractConcord.ContracterIdentifyNumber) ? concord.ContracterIdentifyNumber : exContractConcord.ContracterIdentifyNumber;
        //    concord.ContracterName = string.IsNullOrEmpty(exContractConcord.ContracterName) ? concord.ContracterName : exContractConcord.ContracterName;
        //    concord.ContracterRepresentName = string.IsNullOrEmpty(exContractConcord.ContracterRepresentName) ? concord.ContracterRepresentName : exContractConcord.ContracterRepresentName;
        //    concord.ContracterRepresentNumber = string.IsNullOrEmpty(exContractConcord.ContracterRepresentNumber) ? concord.ContracterRepresentNumber : exContractConcord.ContracterRepresentNumber;
        //    concord.ContracterRepresentTelphone = string.IsNullOrEmpty(exContractConcord.ContracterRepresentTelphone) ? concord.ContracterRepresentTelphone : exContractConcord.ContracterRepresentTelphone;
        //    concord.ContracterRepresentType = exContractConcord.ContracterRepresentType <= 0 ? concord.ContracterRepresentType : ((int)Enum.Parse(typeof(eCredentialsType), exContractConcord.ContracterRepresentType.ToString())).ToString();
        //    concord.ContractMoney = exContractConcord.ContractMoney == 0.0 ? concord.ContractMoney : exContractConcord.ContractMoney;
        //    concord.CountActualArea = exContractConcord.CountActualArea == 0.0 ? concord.CountActualArea : exContractConcord.CountActualArea;
        //    concord.CountAwareArea = exContractConcord.CountAwareArea == 0.0 ? concord.CountAwareArea : exContractConcord.CountAwareArea;
        //    concord.CountMotorizeLandArea = exContractConcord.CountMotorizeLandArea == 0.0 ? concord.CountMotorizeLandArea : exContractConcord.CountMotorizeLandArea;
        //    concord.CreationTime = exContractConcord.CreationTime == null ? concord.CreationTime : exContractConcord.CreationTime;
        //    concord.Flag = (exContractConcord.ArableLandStartTime != null && exContractConcord.ArableLandEndTime != null) ? exContractConcord.Flag : concord.Flag;
        //    concord.Founder = string.IsNullOrEmpty(exContractConcord.Founder) ? concord.Founder : exContractConcord.Founder;
        //    concord.IsValid = true;
        //    concord.LandPurpose = exContractConcord.LandPurpose <= 0 ? concord.LandPurpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractConcord.LandPurpose.ToString())).ToString();
        //    concord.ManagementTime = exContractConcord.ManagementTime == null ? concord.ManagementTime : exContractConcord.ManagementTime;
        //    concord.ManagementTime = concord.ManagementTime == null ? "" : concord.ManagementTime;
        //    concord.ModifiedTime = exContractConcord.ModifiedTime == null ? concord.ModifiedTime : exContractConcord.ModifiedTime;
        //    concord.Modifier = string.IsNullOrEmpty(exContractConcord.Modifier) ? concord.Modifier : exContractConcord.Modifier;
        //    concord.PersonAvgArea = exContractConcord.PersonAvgArea == 0 ? concord.PersonAvgArea : exContractConcord.PersonAvgArea;
        //    concord.PrivateArea = exContractConcord.PrivateArea == 0 ? concord.PrivateArea : exContractConcord.PrivateArea;
        //    concord.RequireBookId = (exContractConcord.RequireBookId == null || exContractConcord.RequireBookId == Guid.Empty) ? concord.RequireBookId : exContractConcord.RequireBookId;
        //    concord.SecondContracterLocated = string.IsNullOrEmpty(exContractConcord.SecondContracterLocated) ? concord.SecondContracterLocated : exContractConcord.SecondContracterLocated;
        //    concord.SecondContracterName = string.IsNullOrEmpty(exContractConcord.SecondContracterName) ? concord.SecondContracterName : exContractConcord.SecondContracterName;
        //    concord.SenderDate = exContractConcord.SenderDate == null ? concord.SenderDate : exContractConcord.SenderDate;
        //    concord.SenderName = string.IsNullOrEmpty(exContractConcord.SenderName) ? concord.SenderName : exContractConcord.SenderName;
        //    concord.SenderId = (exContractConcord.EmployerId == null || exContractConcord.EmployerId == Guid.Empty) ? concord.SenderId : exContractConcord.EmployerId;
        //    concord.Status = exContractConcord.Status <= 0 ? concord.Status : (eStatus)Enum.Parse(typeof(eStatus), exContractConcord.Status.ToString());
        //    concord.TotalTableArea = exContractConcord.TotalTableArea == 0 ? concord.TotalTableArea : exContractConcord.TotalTableArea;
        //    return concord;
        //}

        /// <summary>
        /// 导入登记薄
        /// </summary>
        /// <param name="ecrb"></param>
        //private void ImportAgriContractRegeditBook(YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook ecrb, string zoneCode)
        //{
        //    if (ecrb == null)
        //    {
        //        return;
        //    }
        //    var bookStation = database.CreateRegeditBookStation();
        //    ContractRegeditBook crb = AgriContractRegeditBookMapping(ecrb, zoneCode, bookStation);
        //    crb.ID = crb.ID != Guid.Empty ? crb.ID : ecrb.ID;
        //    if (string.IsNullOrEmpty(crb.Number))
        //    {
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(crb.Year))
        //    {
        //        crb.Year = DateTime.Now.Year.ToString();
        //    }
        //    bookStation.Add(crb);
        //    crb = null;
        //}

        /// <summary>
        /// 登记薄映射
        /// </summary>
        /// <param name="ecrb"></param>
        /// <returns></returns>
        //private ContractRegeditBook AgriContractRegeditBookMapping(YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook ecrb, string zoneCode, IContractRegeditBookWorkStation bookStation)
        //{
        //    ContractRegeditBook crb = bookStation.Get(ecrb.ID);
        //    if (crb == null)
        //    {
        //        crb = bookStation.Get(t => t.RegeditNumber == ecrb.RegeditNumber).FirstOrDefault();
        //    }
        //    if (crb == null)
        //    {
        //        crb = new ContractRegeditBook();
        //        crb.ID = Guid.Empty;
        //    }
        //    crb.Comment = string.IsNullOrEmpty(ecrb.Comment) ? crb.Comment : ecrb.Comment;
        //    crb.Count = ecrb.Count <= 0 ? crb.Count : ecrb.Count;
        //    crb.CreationTime = ecrb.CreationTime == null ? crb.CreationTime : ecrb.CreationTime;
        //    crb.Founder = string.IsNullOrEmpty(ecrb.Founder) ? crb.Founder : ecrb.Founder;
        //    crb.ModifiedTime = ecrb.ModifiedTime == null ? crb.ModifiedTime : ecrb.ModifiedTime;
        //    crb.Modifier = string.IsNullOrEmpty(ecrb.Modifier) ? crb.Modifier : ecrb.Modifier;
        //    crb.Number = string.IsNullOrEmpty(ecrb.Number) ? ecrb.RegeditNumber : ecrb.Number;
        //    crb.RegeditNumber = !string.IsNullOrEmpty(ecrb.Number) ? ecrb.Number : ecrb.RegeditNumber;
        //    if (!string.IsNullOrEmpty(ecrb.SerialNumber))
        //    {
        //        crb.Number = ecrb.SerialNumber;
        //    }
        //    crb.PrintDate = (ecrb.PrintDate == null && !ecrb.PrintDate.HasValue) ? crb.PrintDate : ecrb.PrintDate.Value;
        //    if (ecrb.SendDate != null)
        //    {
        //        crb.SendDate = ecrb.SendDate;
        //    }
        //    crb.SendOrganization = string.IsNullOrEmpty(ecrb.SendOrganization) ? crb.SendOrganization : ecrb.SendOrganization;
        //    if (ecrb.WriteDate != null)
        //    {
        //        crb.WriteDate = ecrb.WriteDate;
        //    }
        //    crb.WriteOrganization = string.IsNullOrEmpty(ecrb.WriteOrganization) ? crb.WriteOrganization : ecrb.WriteOrganization;
        //    crb.Year = string.IsNullOrEmpty(ecrb.Year) ? crb.Year : ecrb.Year;
        //    crb.ZoneCode = zoneCode;
        //    return crb;
        //}

        /// <summary>
        /// 导入地块
        /// </summary>
        /// <param name="landCollection"></param>
        //private void ImportAgriContractLands(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection landCollection = landContractor.ContractLands;
        //    if (landCollection == null || landCollection.Count == 0)
        //    {
        //        return;
        //    }
        //    var landStation = database.CreateContractLandWorkstation();
        //    foreach (YuLinTu.Business.ContractLand.Exchange2.ExContractLand land in landCollection)
        //    {
        //        ContractLand contractLand = AgriContractLandMapping(land, true);
        //        contractLand.ID = contractLand.ID != Guid.Empty ? contractLand.ID : land.ID;
        //        contractLand.OwnerId = landContractor.Contractor != null ? landContractor.Contractor.ID : Guid.Empty;
        //        if (string.IsNullOrEmpty(contractLand.LocationCode))
        //        {
        //            contractLand.LocationCode = landContractor.Zone != null ? landContractor.Zone.FullCode : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.LocationName))
        //        {
        //            contractLand.LocationName = landContractor.Zone != null ? landContractor.Zone.FullName : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.ZoneCode))
        //        {
        //            contractLand.ZoneCode = landContractor.Zone != null ? landContractor.Zone.FullCode : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.ZoneName))
        //        {
        //            contractLand.ZoneName = landContractor.Zone != null ? landContractor.Zone.FullName : string.Empty;
        //        }
        //        try
        //        {
        //            if (contractLand.ZoneCode.Length == 16)
        //            {
        //                contractLand.ZoneCode = contractLand.ZoneCode.Substring(0, 12) + contractLand.ZoneCode.Substring(14, 2);
        //            }
        //            if (land.Shape != null)
        //            {
        //                contractLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(land.Shape.WellKnownBinary, 0);
        //            }
        //            else
        //            {
        //                string information = string.Format("承包方:{0}下地块编码为{1}的地块没有空间信息!", land.HouseHolderName, ContractLand.GetLandNumber(land.CadastralNumber));
        //                this.ReportAlert(eMessageGrade.Infomation, null, information);
        //            }
        //            landStation.Add(contractLand);
        //            contractLand = null;
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.ToString());
        //        }
        //    }
        //}

        /// <summary>
        /// 地块映射
        /// </summary>
        /// <param name="exContractLand"></param>
        /// <returns></returns>
        //private ContractLand AgriContractLandMapping(YuLinTu.Business.ContractLand.Exchange2.ExContractLand exContractLand, bool isImport)
        //{
        //    ContractLand land = landEntity.Lands.Find(ld => ld.ID == exContractLand.ID);
        //    if (land == null && isImport)
        //    {
        //        land = landEntity.Lands.Find(ld => ld.CadastralNumber == exContractLand.CadastralNumber);
        //    }
        //    if (land == null)
        //    {
        //        land = new ContractLand();
        //        land.ID = Guid.Empty;
        //    }
        //    land.ConcordId = (exContractLand.ConcordId == null || exContractLand.ConcordId == Guid.Empty) ? land.ConcordId : exContractLand.ConcordId;
        //    land.ActualArea = exContractLand.ActualArea == 0.0 ? land.ActualArea : exContractLand.ActualArea;
        //    land.AwareArea = exContractLand.AwareArea == 0.0 ? land.AwareArea : exContractLand.AwareArea;
        //    string fixNumber = currentZone.Level == eZoneLevel.Village ? currentZone.FullCode + "0000000" : currentZone.FullCode + "000";
        //    land.CadastralNumber = string.IsNullOrEmpty(exContractLand.CadastralNumber) ? fixNumber + exContractLand.AgricultureNumber : exContractLand.CadastralNumber;
        //    land.Comment = string.IsNullOrEmpty(exContractLand.Comment) ? land.Comment : exContractLand.Comment;
        //    land.ConstructMode = exContractLand.ContractType <= 0 ? land.ConstructMode : ((int)Enum.Parse(typeof(eContracterType), exContractLand.ContractType.ToString())).ToString();
        //    land.CreationTime = exContractLand.CreationTime == null ? land.CreationTime : exContractLand.CreationTime;
        //    land.FormerPerson = string.IsNullOrEmpty(exContractLand.FormerPerson) ? land.FormerPerson : exContractLand.FormerPerson;
        //    land.Founder = string.IsNullOrEmpty(exContractLand.Founder) ? land.Founder : exContractLand.Founder;
        //    land.OwnerName = string.IsNullOrEmpty(exContractLand.HouseHolderName) ? land.OwnerName : exContractLand.HouseHolderName;
        //    land.IsFarmerLand = (exContractLand.IsFarmerLand != null && exContractLand.IsFarmerLand.HasValue) ? exContractLand.IsFarmerLand.Value : land.IsFarmerLand;
        //    land.IsFlyLand = exContractLand.IsFlyLand;
        //    land.IsTransfer = exContractLand.IsTransfer;
        //    land.LandCode = string.IsNullOrEmpty(exContractLand.LandCode) ? land.LandCode : exContractLand.LandCode;
        //    land.LandLevel = exContractLand.LandLevel <= 0 ? land.LandLevel : ((int)Enum.Parse(typeof(eContractLandLevel), exContractLand.LandLevel.ToString())).ToString();
        //    if (!string.IsNullOrEmpty(exContractLand.LandNeighbor))
        //    {
        //        string[] neighbor = exContractLand.LandNeighbor.Split('#');
        //        land.NeighborEast = neighbor[0];
        //        land.NeighborSouth = neighbor[1];
        //        land.NeighborWest = neighbor[2];
        //        land.NeighborNorth = neighbor[3];
        //    }
        //    land.LandNumber = string.IsNullOrEmpty(exContractLand.LandNumber) ? land.LandNumber : exContractLand.LandNumber;
        //    land.LandScopeLevel = exContractLand.LandScopeLevel <= 0 ? land.LandScopeLevel : ((int)Enum.Parse(typeof(eLandSlopeLevel), exContractLand.LandScopeLevel.ToString())).ToString();
        //    land.LineArea = exContractLand.LineArea == 0.0 ? land.LineArea : exContractLand.LineArea;
        //    land.ManagementType = exContractLand.ManagementType <= 0 ? land.ManagementType : ((int)Enum.Parse(typeof(eManageType), exContractLand.ManagementType.ToString())).ToString();
        //    land.ModifiedTime = exContractLand.ModifiedTime == null ? land.ModifiedTime : exContractLand.ModifiedTime;
        //    land.Modifier = string.IsNullOrEmpty(exContractLand.Modifier) ? land.Modifier : exContractLand.Modifier;
        //    land.MotorizeLandArea = exContractLand.MotorizeLandArea == 0.0 ? land.MotorizeLandArea : exContractLand.MotorizeLandArea;
        //    land.Name = string.IsNullOrEmpty(exContractLand.Name) ? land.Name : exContractLand.Name;
        //    land.ZoneName = string.IsNullOrEmpty(exContractLand.OwnerRightName) ? land.ZoneName : exContractLand.OwnerRightName;
        //    land.OwnRightType = exContractLand.OwnerRightType <= 0 ? land.OwnRightType : ((int)Enum.Parse(typeof(eLandPropertyType), exContractLand.OwnerRightType.ToString())).ToString();
        //    land.PertainToArea = exContractLand.PertainToArea == 0.0 ? land.PertainToArea : exContractLand.PertainToArea;
        //    land.PlantType = exContractLand.PlantType <= 0 ? land.PlantType : ((int)Enum.Parse(typeof(ePlantProtectType), exContractLand.PlantType.ToString())).ToString();
        //    land.PlotNumber = string.IsNullOrEmpty(exContractLand.PlotNumber) ? land.PlotNumber : exContractLand.PlotNumber;
        //    land.Purpose = exContractLand.Purpose <= 0 ? land.Purpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractLand.Purpose.ToString())).ToString();
        //    land.Soiltype = string.IsNullOrEmpty(exContractLand.Soiltype) ? land.Soiltype : exContractLand.Soiltype;
        //    land.TableArea = exContractLand.TableArea == 0.0 ? land.TableArea : exContractLand.TableArea;
        //    land.TransferType = exContractLand.TransferType <= 0 ? land.TransferType : ((int)Enum.Parse(typeof(eTransferType), exContractLand.TransferType.ToString())).ToString();
        //    land.WidthHeight = string.IsNullOrEmpty(exContractLand.WidthHeight) ? land.WidthHeight : exContractLand.WidthHeight;
        //    land.LandName = string.IsNullOrEmpty(exContractLand.LandName) ? land.LandName : exContractLand.LandName;
        //    land.PlatType = exContractLand.PlatType <= 0 ? land.PlatType : ((int)Enum.Parse(typeof(ePlantingType), exContractLand.PlatType.ToString())).ToString();
        //    if (string.IsNullOrEmpty(land.LandNumber))
        //    {
        //        land.LandNumber = ContractLand.GetLandNumber(land.CadastralNumber);
        //    }
        //    land.ExtendA = string.IsNullOrEmpty(exContractLand.Extend1) ? land.ExtendB : exContractLand.Extend1;
        //    land.ExtendB = string.IsNullOrEmpty(exContractLand.ParcelNumber) ? land.ExtendB : exContractLand.ParcelNumber;
        //    land.ExtendC = string.IsNullOrEmpty(exContractLand.Extend2) ? land.ExtendB : exContractLand.Extend2;
        //    InitalizeLandExpandInformation(land, exContractLand);
        //    return land;
        //}

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        //private void InitalizeLandExpandInformation(ContractLand land, YuLinTu.Business.ContractLand.Exchange2.ExContractLand exContractLand)
        //{
        //    AgricultureLandExpand landExpand = new AgricultureLandExpand();
        //    landExpand.ID = land.ID;
        //    landExpand.Name = land.Name;
        //    landExpand.HouseHolderName = land.OwnerName;
        //    landExpand.AgricultureNumber = exContractLand.AgricultureNumber;
        //    landExpand.UseSituation = exContractLand.Extend7;
        //    landExpand.Yield = exContractLand.Extend8;
        //    landExpand.OutputValue = exContractLand.Extend9;
        //    landExpand.IncomeSituation = exContractLand.Extend10;
        //    object value = null;
        //    double elevation = 0.0;
        //    if (exContractLand.LandExpand == null)
        //    {
        //        exContractLand.LandExpand = new Dictionary<string, object>();
        //    }
        //    exContractLand.LandExpand.TryGetValue("Elevation", out value);
        //    if (value != null && !string.IsNullOrEmpty(value.ToString()))
        //    {
        //        double.TryParse(value.ToString(), out elevation);
        //    }
        //    landExpand.Elevation = elevation;
        //    land.LandExpand = landExpand;//扩展信息
        //    if (string.IsNullOrEmpty(land.CadastralNumber))
        //    {
        //        land.CadastralNumber = exContractLand.AgricultureNumber;
        //    }
        //}

        /// <summary>
        /// 导入申请书
        /// </summary>
        /// <param name="ecrb"></param>
        //private void ImportContractRequireBook(YuLinTu.Business.ContractLand.Exchange2.ExRegisterApplication application, YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord, string zoneCode)
        //{
        //    //if (application == null || exContractConcord.RequireBookId == Guid.Empty || exContractConcord.RequireBookId == null
        //    //    || database.ContractRequireTable.Get(exContractConcord.RequireBookId) != null)
        //    //{
        //    //    return;
        //    //}
        //    //ContractRequireTable table = new ContractRequireTable();
        //    //table.ID = exContractConcord.RequireBookId;
        //    //table.Year = application.Year;
        //    //table.Number = application.Number;
        //    //table.Person = application.Applicant;
        //    //table.Date = DateTime.Now;
        //    //table.CreationTime = DateTime.Now;
        //    //table.ModifiedTime = DateTime.Now;
        //    //table.Modifier = "Admin";
        //    //table.Founder = "Admin";
        //    //table.ZoneCode = zoneCode;
        //    //database.ContractRequireTable.Add(table);
        //    //table = null;
        //}

        /// <summary>
        /// 发包方映射
        /// </summary>
        //private CollectivityTissue AgriSenderMapping(YuLinTu.Business.ContractLand.Exchange2.ExCollectiveTissue exTissue)
        //{
        //    if (exTissue == null)
        //    {
        //        return null;
        //    }
        //    var tissueStation = database.CreateSenderWorkStation();
        //    CollectivityTissue tissue = tissueStation.Get(exTissue.ID);
        //    if (tissue == null)
        //    {
        //        tissue = tissueStation.Get(t => t.Code == exTissue.Code).FirstOrDefault();
        //    }
        //    if (tissue == null)
        //    {
        //        tissue = new CollectivityTissue();
        //        tissue.ID = Guid.Empty;
        //    }
        //    tissue.ID = tissue.ID != Guid.Empty ? tissue.ID : exTissue.ID;
        //    tissue.Code = string.IsNullOrEmpty(exTissue.Code) ? tissue.Code : exTissue.Code;
        //    tissue.LawyerCartNumber = string.IsNullOrEmpty(exTissue.LawyerCartNumber) ? tissue.LawyerCartNumber : exTissue.LawyerCartNumber;
        //    tissue.LawyerName = string.IsNullOrEmpty(exTissue.LawyerName) ? tissue.LawyerName : exTissue.LawyerName;
        //    tissue.Name = string.IsNullOrEmpty(exTissue.Name) ? tissue.Name : exTissue.Name;
        //    tissue.Type = exTissue.Type <= 0 ? tissue.Type : (eTissueType)Enum.Parse(typeof(eTissueType), exTissue.Type.ToString());
        //    tissue.ZoneCode = string.IsNullOrEmpty(exTissue.ZoneCode) ? tissue.ZoneCode : exTissue.ZoneCode;
        //    return tissue;
        //}

        /// <summary>
        /// 导入集体经济组织
        /// </summary>
        /// <param name="exContractConcord"></param>
        //private void ImportAgriSender(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    if (landContractor == null)
        //    {
        //        return;
        //    }
        //    YuLinTu.Business.ContractLand.Exchange2.ExCollectiveTissue exTissue = landContractor.Contractee;
        //    if (exTissue == null || string.IsNullOrEmpty(exTissue.Name))
        //    {
        //        return;
        //    }
        //    var tissueStation = database.CreateSenderWorkStation();
        //    CollectivityTissue tissue = AgriSenderMapping(exTissue);
        //    if (tissue == null)
        //    {
        //        return;
        //    }
        //    if (SenderList.Contains(tissue.Name))
        //    {
        //        return;
        //    }
        //    SenderList.Add(tissue.Name);
        //    CollectivityTissue tue = tissueStation.Get(tissue.ID);
        //    if (tue != null)
        //    {
        //        return;
        //    }
        //    tue = tissueStation.Get(tissue.Name);
        //    if (tue != null && currentZone.FullCode.IndexOf(tue.ZoneCode) == 0)
        //    {
        //        return;
        //    }
        //    tissueStation.Add(tissue);
        //    tissue = null;
        //    tue = null;
        //}

        #endregion

        //#region Methods-Update

        ///// <summary>
        ///// 导入承包方
        ///// </summary>
        ///// <param name="exContractor"></param>
        //private bool UpdateAgriContractor(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        //{
        //    if (exLandContractor == null)
        //    {
        //        return false;
        //    }
        //    VirtualPerson virtualPerson = AgriContractorMapping(exLandContractor);
        //    if (virtualPerson.Status == eVirtualPersonStatus.Lock)
        //    {
        //        return false;
        //    }
        //    var personStation = database.CreateVirtualPersonStation<LandVirtualPerson>();
        //    if (virtualPerson.ID == Guid.Empty)
        //    {
        //        virtualPerson.ID = virtualPerson.ID == Guid.Empty ? exLandContractor.Contractor.ID : virtualPerson.ID;
        //        personStation.Add(virtualPerson);
        //        virtualPerson = null;
        //        return true;
        //    }
        //    virtualPerson.ID = virtualPerson.ID == Guid.Empty ? exLandContractor.Contractor.ID : virtualPerson.ID;
        //    var p = personStation.FirstOrDefault(t => t.ID == virtualPerson.ID || (t.ZoneCode == virtualPerson.ZoneCode && t.Name == virtualPerson.Name));
        //    if (p != null)
        //    {
        //        virtualPerson.ID = p.ID;
        //        personStation.Update(virtualPerson);
        //    }
        //    else
        //    {
        //        personStation.Add(virtualPerson);
        //    }
        //    virtualPerson = null;
        //    return true;
        //}

        ///// <summary>
        ///// 更新承包合同
        ///// </summary>
        ///// <param name="exContractConcord"></param>
        //private void UpdateAgriContractConcord(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    if (landContractor == null)
        //    {
        //        return;
        //    }
        //    YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord = landContractor.ContractConcord;
        //    if (exContractConcord == null || string.IsNullOrEmpty(exContractConcord.ConcordNumber))
        //    {
        //        return;
        //    }
        //    var concordStation = database.CreateConcordStation();
        //    ContractConcord concord = AgriContractConcordMapping(exContractConcord);
        //    concord.ZoneCode = landContractor.Zone != null ? landContractor.Zone.FullCode : currentZone.FullCode;
        //    if (concord.ID == Guid.Empty)
        //    {
        //        concord.ID = concord.ID != Guid.Empty ? concord.ID : exContractConcord.ID;
        //        concordStation.Add(concord);
        //        return;
        //    }
        //    var concordStaion = database.CreateConcordStation();
        //    concord.ID = concord.ID != Guid.Empty ? concord.ID : exContractConcord.ID;

        //    var c = concordStaion.FirstOrDefault(t => t.ID == concord.ID || (t.ConcordNumber == concord.ConcordNumber));
        //    if (c != null)
        //    {
        //        concord.ID = c.ID;
        //        concordStaion.Update(concord);
        //    }
        //    else
        //    {
        //        concordStaion.Add(concord);
        //    }
        //    concord = null;
        //}

        ///// <summary>
        ///// 导入登记薄
        ///// </summary>
        ///// <param name="ecrb"></param>
        //private void UpdateAgriContractRegeditBook(YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook ecrb, string zoneCode)
        //{
        //    if (ecrb == null)
        //    {
        //        return;
        //    }
        //    var bookStation = database.CreateRegeditBookStation();
        //    ContractRegeditBook crb = AgriContractRegeditBookMapping(ecrb, zoneCode, bookStation);
        //    if (crb.ID == Guid.Empty)
        //    {
        //        crb.ID = crb.ID != Guid.Empty ? crb.ID : ecrb.ID;
        //        bookStation.Add(crb);
        //        return;
        //    }
        //    crb.ID = crb.ID != Guid.Empty ? crb.ID : ecrb.ID;
        //    var c = bookStation.FirstOrDefault(t => t.ID == crb.ID || (t.RegeditNumber == crb.RegeditNumber));
        //    if (c != null)
        //    {
        //        crb.ID = c.ID;
        //        bookStation.Update(crb);
        //    }
        //    else
        //    {
        //        bookStation.Add(crb);
        //    }
        //    crb = null;
        //}

        ///// <summary>
        ///// 导入地块
        ///// </summary>
        ///// <param name="landCollection"></param>
        //private void UpdateAgriContractLands(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection landCollection = landContractor.ContractLands;
        //    if (landCollection == null || landCollection.Count == 0)
        //    {
        //        return;
        //    }
        //    VirtualPerson vp = landEntity.Contractors.Find(fam => fam.ID == landContractor.Contractor.ID);
        //    if (vp == null)
        //    {
        //        vp = landEntity.Contractors.Find(fam => fam.Name == landContractor.Contractor.Name);
        //    }
        //    bool checkLandNumber = true;
        //    string value = ToolConfiguration.GetSpecialAppSettingValue("CheckLandNumberWithImportData", "false");
        //    Boolean.TryParse(value, out checkLandNumber);
        //    foreach (YuLinTu.Business.ContractLand.Exchange2.ExContractLand land in landCollection)
        //    {
        //        ContractLand contractLand = AgriContractLandMapping(land, true);
        //        contractLand.OwnerId = vp != null ? vp.ID : landContractor.Contractor.ID;
        //        if (string.IsNullOrEmpty(contractLand.LocationCode))
        //        {
        //            contractLand.LocationCode = landContractor.Zone != null ? landContractor.Zone.FullCode : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.LocationName))
        //        {
        //            contractLand.LocationName = landContractor.Zone != null ? landContractor.Zone.FullName : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.ZoneCode))
        //        {
        //            contractLand.ZoneCode = landContractor.Zone != null ? landContractor.Zone.FullCode : string.Empty;
        //        }
        //        if (string.IsNullOrEmpty(contractLand.ZoneName))
        //        {
        //            contractLand.ZoneName = landContractor.Zone != null ? landContractor.Zone.FullName : string.Empty;
        //        }
        //        try
        //        {
        //            var landsStation = database.CreateContractLandWorkstation();
        //            contractLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(land.Shape.WellKnownBinary, 0);
        //            if (contractLand.ID == Guid.Empty)
        //            {
        //                contractLand.ID = contractLand.ID != Guid.Empty ? contractLand.ID : land.ID;
        //                landsStation.Add(contractLand);
        //            }
        //            else
        //            {
        //                contractLand.ID = contractLand.ID != Guid.Empty ? contractLand.ID : land.ID;
        //                var en = landsStation.FirstOrDefault(t => t.ID == contractLand.ID || (t.ZoneCode == contractLand.ZoneCode && t.LandNumber == contractLand.LandNumber));
        //                if (en != null)
        //                {
        //                    contractLand.ID = en.ID;
        //                    landsStation.Update(contractLand);
        //                }
        //                else { landsStation.Add(contractLand); }
        //                contractLand = null;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.ToString());
        //        }
        //    }
        //    vp = null;
        //}

        ///// <summary>
        ///// 导入申请书
        ///// </summary>
        ///// <param name="ecrb"></param>
        //private void UpdateContractRequireBook(YuLinTu.Business.ContractLand.Exchange2.ExRegisterApplication application, YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord, string zoneCode)
        //{
        //    var requireStation = database.CreateRequireTableWorkStation();
        //    var require = requireStation.Get(exContractConcord.RequireBookId);
        //    if (application == null || exContractConcord.RequireBookId == Guid.Empty || exContractConcord.RequireBookId == null
        //        || require != null)
        //    {
        //        return;
        //    }
        //    ContractRequireTable table = require;
        //    if (table == null)
        //    {
        //        table = new ContractRequireTable();
        //        table.ID = Guid.Empty;
        //    }
        //    table.Year = string.IsNullOrEmpty(application.Year) ? table.Year : application.Year;
        //    table.Number = string.IsNullOrEmpty(application.Number) ? table.Number : application.Number;
        //    table.Person = string.IsNullOrEmpty(application.Applicant) ? table.Person : application.Applicant;
        //    table.Date = DateTime.Now;
        //    table.CreationTime = DateTime.Now;
        //    table.ModifiedTime = DateTime.Now;
        //    table.Modifier = "Admin";
        //    table.Founder = "Admin";
        //    table.ZoneCode = zoneCode;
        //    if (table.ID == Guid.Empty)
        //    {
        //        table.ID = table.ID != Guid.Empty ? table.ID : exContractConcord.RequireBookId;
        //        requireStation.Add(table);
        //    }
        //    table.ID = table.ID != Guid.Empty ? table.ID : exContractConcord.RequireBookId;
        //    if (requireStation.Update(table) <= 0)
        //    {
        //        requireStation.Add(table);
        //    }
        //    table = null;
        //}

        ///// <summary>
        ///// 导入集体经济组织
        ///// </summary>
        ///// <param name="exContractConcord"></param>
        //private void UpdateAgriSender(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        //{
        //    if (landContractor == null)
        //    {
        //        return;
        //    }
        //    YuLinTu.Business.ContractLand.Exchange2.ExCollectiveTissue exTissue = landContractor.Contractee;
        //    if (exTissue == null || string.IsNullOrEmpty(exTissue.Name))
        //    {
        //        return;
        //    }
        //    CollectivityTissue tissue = AgriSenderMapping(exTissue);
        //    if (tissue == null)
        //    {
        //        return;
        //    }
        //    if (SenderList.Contains(tissue.Name))
        //    {
        //        return;
        //    }
        //    SenderList.Add(tissue.Name);
        //    var tissueStation = database.CreateSenderWorkStation();
        //    CollectivityTissue tue = tissueStation.Get(tissue.ID);
        //    if (tue != null)
        //    {
        //        return;
        //    }
        //    tue = tissueStation.Get(tissue.Name);
        //    if (tue != null && currentZone.FullCode.IndexOf(tue.ZoneCode) == 0)
        //    {
        //        return;
        //    }
        //    tissueStation.Add(tissue);
        //    tue = null;
        //    tissue = null;
        //}

        //#endregion

        #endregion
    }
}
