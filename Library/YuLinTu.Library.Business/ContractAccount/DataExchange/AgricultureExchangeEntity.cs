/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using YuLinTu.Library.Entity;
using System.Collections.Generic;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 复合交换实体
    /// </summary>
    public class AgricultureExchangeEntity
    {
        #region Propertys

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public ContractConcord Concord { get; set; }

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneCollection { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }

        #endregion

        #region Ctor

        public AgricultureExchangeEntity()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 设置地域信息
        /// </summary>
        public void SetArcZoneInformation(YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection zoneCollection)
        {
            ZoneCollection = new List<Zone>();
            foreach (var exZone in zoneCollection)
            {
                Zone arcZone = ArcZoneMapping(exZone);
                if (ZoneCollection.Find(az => az.ID == arcZone.ID || az.FullCode == arcZone.FullCode) != null)
                {
                    continue;
                }
                ZoneCollection.Add(arcZone);
            }
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        public void SetArcLandInformation(YuLinTu.Business.ContractLand.Exchange2.ExContractLand[] landCollection)
        {
            LandCollection = new List<ContractLand>();
            Zone arcZone = null;
            if (ZoneCollection != null)
            {
                if (ZoneCollection.Count == 1)
                {
                    arcZone = ZoneCollection[0];
                }
                else
                {
                    arcZone = ZoneCollection.Find(az => az.Level == eZoneLevel.Group);
                }
            }
            foreach (var exContractLand in landCollection)
            {
                ContractLand arcLand = ArcContractLandMapping(exContractLand);
                if (LandCollection.Find(ad => ad.ID == arcLand.ID || ad.CadastralNumber == arcLand.CadastralNumber) != null)
                {
                    continue;
                }
                if (arcZone != null)
                {
                    arcLand.ZoneCode = arcZone.FullCode;
                    arcLand.ZoneName = arcZone.FullName;
                }
                LandCollection.Add(arcLand);
            }
        }

        /// <summary>
        /// 承包合同映射
        /// </summary>
        /// <param name="exContractConcord"></param>
        /// <returns></returns>
        public void SetArcContractConcordMapping(YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exContractConcord)
        {
            Concord = new ContractConcord();
            Concord.ID = exContractConcord.ID;
            Concord.ConcordNumber = string.IsNullOrEmpty(exContractConcord.ConcordNumber) ? exContractConcord.AgriConcordNumber : exContractConcord.ConcordNumber;
            Concord.AgentCrdentialType = exContractConcord.AgentCrdentialType <= 0 ? Concord.AgentCrdentialType : ((int)Enum.Parse(typeof(eCredentialsType), exContractConcord.AgentCrdentialType.ToString())).ToString();
            Concord.AgentName = string.IsNullOrEmpty(exContractConcord.AgentName) ? Concord.AgentName : exContractConcord.AgentName;
            Concord.AgentTelphone = string.IsNullOrEmpty(exContractConcord.AgentTelphone) ? Concord.AgentTelphone : exContractConcord.AgentTelphone;
            Concord.ArableLandEndTime = exContractConcord.ArableLandEndTime == null ? Concord.ArableLandEndTime : exContractConcord.ArableLandEndTime;
            Concord.ArableLandStartTime = exContractConcord.ArableLandStartTime == null ? Concord.ArableLandStartTime : exContractConcord.ArableLandStartTime;
            Concord.ArableLandType = exContractConcord.ArableLandType <= 0 ? Concord.ArableLandType : ((int)Enum.Parse(typeof(eConstructMode), exContractConcord.ArableLandType.ToString())).ToString();
            Concord.BadlandEndTime = exContractConcord.BadlandEndTime == null ? Concord.BadlandEndTime : exContractConcord.BadlandEndTime;
            Concord.BadlandPurpose = exContractConcord.BadlandPurpose <= 0 ? Concord.BadlandPurpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractConcord.BadlandPurpose.ToString())).ToString();
            Concord.BadlandStartTime = exContractConcord.BadlandStartTime == null ? Concord.BadlandStartTime : exContractConcord.BadlandStartTime;
            Concord.BadlandType = exContractConcord.BadlandType <= 0 ? Concord.BadlandType : ((int)Enum.Parse(typeof(eConstructMode), exContractConcord.BadlandType.ToString())).ToString();
            Concord.CheckAgencyDate = exContractConcord.CheckAgencyDate == null ? Concord.CheckAgencyDate : exContractConcord.CheckAgencyDate;
            Concord.Comment = string.IsNullOrEmpty(exContractConcord.Comment) ? Concord.Comment : exContractConcord.Comment;
            Concord.ContracerType = exContractConcord.ContracerType <= 0 ? Concord.ContracerType : ((int)Enum.Parse(typeof(eContractorType), exContractConcord.ContracerType.ToString())).ToString();
            Concord.ContractCredentialNumber = string.IsNullOrEmpty(exContractConcord.ContractCredentialNumber) ? Concord.ContractCredentialNumber : exContractConcord.ContractCredentialNumber;
            Concord.ContractDate = exContractConcord.ContractDate == null ? Concord.ContractDate : exContractConcord.ContractDate;
            if (exContractConcord.ContractorId != null)
            {
                Concord.ContracterId = exContractConcord.ContractorId;
            }
            Concord.ContracterIdentifyNumber = string.IsNullOrEmpty(exContractConcord.ContracterIdentifyNumber) ? Concord.ContracterIdentifyNumber : exContractConcord.ContracterIdentifyNumber;
            Concord.ContracterName = string.IsNullOrEmpty(exContractConcord.ContracterName) ? Concord.ContracterName : exContractConcord.ContracterName;
            Concord.ContracterRepresentName = string.IsNullOrEmpty(exContractConcord.ContracterRepresentName) ? Concord.ContracterRepresentName : exContractConcord.ContracterRepresentName;
            Concord.ContracterRepresentNumber = string.IsNullOrEmpty(exContractConcord.ContracterRepresentNumber) ? Concord.ContracterRepresentNumber : exContractConcord.ContracterRepresentNumber;
            Concord.ContracterRepresentTelphone = string.IsNullOrEmpty(exContractConcord.ContracterRepresentTelphone) ? Concord.ContracterRepresentTelphone : exContractConcord.ContracterRepresentTelphone;
            Concord.ContracterRepresentType = exContractConcord.ContracterRepresentType <= 0 ? Concord.ContracterRepresentType : ((int)Enum.Parse(typeof(eCredentialsType), exContractConcord.ContracterRepresentType.ToString())).ToString();
            Concord.ContractMoney = exContractConcord.ContractMoney == 0.0 ? Concord.ContractMoney : exContractConcord.ContractMoney;
            Concord.CountActualArea = exContractConcord.CountActualArea == 0.0 ? Concord.CountActualArea : exContractConcord.CountActualArea;
            Concord.CountAwareArea = exContractConcord.CountAwareArea == 0.0 ? Concord.CountAwareArea : exContractConcord.CountAwareArea;
            Concord.CountMotorizeLandArea = exContractConcord.CountMotorizeLandArea == 0.0 ? Concord.CountMotorizeLandArea : exContractConcord.CountMotorizeLandArea;
            Concord.CreationTime = exContractConcord.CreationTime == null ? Concord.CreationTime : exContractConcord.CreationTime;
            Concord.Flag = (exContractConcord.ArableLandStartTime != null && exContractConcord.ArableLandEndTime != null) ? exContractConcord.Flag : Concord.Flag;
            Concord.Founder = string.IsNullOrEmpty(exContractConcord.Founder) ? Concord.Founder : exContractConcord.Founder;
            Concord.IsValid = true;
            Concord.LandPurpose = exContractConcord.LandPurpose <= 0 ? Concord.LandPurpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractConcord.LandPurpose.ToString())).ToString();
            Concord.ManagementTime = exContractConcord.ManagementTime == null ? Concord.ManagementTime : exContractConcord.ManagementTime;
            Concord.ManagementTime = Concord.ManagementTime == null ? "" : Concord.ManagementTime;
            Concord.ModifiedTime = exContractConcord.ModifiedTime == null ? Concord.ModifiedTime : exContractConcord.ModifiedTime;
            Concord.Modifier = string.IsNullOrEmpty(exContractConcord.Modifier) ? Concord.Modifier : exContractConcord.Modifier;
            Concord.PersonAvgArea = exContractConcord.PersonAvgArea == 0 ? Concord.PersonAvgArea : exContractConcord.PersonAvgArea;
            Concord.PrivateArea = exContractConcord.PrivateArea == 0 ? Concord.PrivateArea : exContractConcord.PrivateArea;
            Concord.RequireBookId = (exContractConcord.RequireBookId == null || exContractConcord.RequireBookId == Guid.Empty) ? Concord.RequireBookId : exContractConcord.RequireBookId;
            Concord.SecondContracterLocated = string.IsNullOrEmpty(exContractConcord.SecondContracterLocated) ? Concord.SecondContracterLocated : exContractConcord.SecondContracterLocated;
            Concord.SecondContracterName = string.IsNullOrEmpty(exContractConcord.SecondContracterName) ? Concord.SecondContracterName : exContractConcord.SecondContracterName;
            Concord.SenderDate = exContractConcord.SenderDate == null ? Concord.SenderDate : exContractConcord.SenderDate;
            Concord.SenderName = string.IsNullOrEmpty(exContractConcord.SenderName) ? Concord.SenderName : exContractConcord.SenderName;
            Concord.SenderId = (exContractConcord.EmployerId == null || exContractConcord.EmployerId == Guid.Empty) ? Concord.SenderId : exContractConcord.EmployerId;
            Concord.Status = exContractConcord.Status <= 0 ? Concord.Status : (eStatus)Enum.Parse(typeof(eStatus), exContractConcord.Status.ToString());
            Concord.TotalTableArea = exContractConcord.TotalTableArea == 0 ? Concord.TotalTableArea : exContractConcord.TotalTableArea;
        }

        /// <summary>
        /// 承包方映射
        /// </summary>
        /// <param name="exContractor"></param>
        /// <returns></returns>
        public void SetArcContractorMapping(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExContractor exContractor = exLandContractor.Contractor;
            if (exContractor == null)
            {
                return;
            }
            Contractor = new VirtualPerson();
            Contractor.ID = exContractor.ID;
            Contractor.PostalNumber = string.IsNullOrEmpty(exContractor.PostNumber) ? Contractor.PostalNumber : exContractor.PostNumber;
            Contractor.FamilyNumber = string.IsNullOrEmpty(exContractor.FamilyNumber) ? Contractor.FamilyNumber : InitalizeFamilyNumber(exContractor.FamilyNumber);
            Contractor.Comment = string.IsNullOrEmpty(exContractor.Comment) ? Contractor.Comment : exContractor.Comment;
            Contractor.CreationTime = exContractor.CreationTime == null ? Contractor.CreationTime : exContractor.CreationTime;
            Contractor.Founder = string.IsNullOrEmpty(exContractor.Founder) ? Contractor.Founder : exContractor.Founder;
            Contractor.ModifiedTime = exContractor.ModifiedTime == null ? DateTime.Now : exContractor.ModifiedTime;
            Contractor.Modifier = string.IsNullOrEmpty(exContractor.Modifier) ? Contractor.Modifier : exContractor.Modifier;
            Contractor.Name = string.IsNullOrEmpty(exContractor.Name) ? Contractor.Name : exContractor.Name;
            Contractor.Number = string.IsNullOrEmpty(exContractor.Number) ? Contractor.Number : exContractor.Number;
            Contractor.Telephone = string.IsNullOrEmpty(exContractor.Telephone) ? Contractor.Telephone : exContractor.Telephone;
            Contractor.VirtualType = eVirtualPersonType.Family;
            Contractor.ZoneCode = exLandContractor.Zone.FullCode;
            Contractor.Address = string.IsNullOrEmpty(exContractor.LandLocated) ? Contractor.Address : exContractor.LandLocated;
            Contractor.SharePersonList = GetAgriSharePersons(exLandContractor);
        }

        /// <summary>
        /// 获取共有人信息
        /// </summary>
        private List<Person> GetAgriSharePersons(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor exLandContractor)
        {
            List<Person> sharePersons = new List<Person>();
            YuLinTu.Business.ContractLand.Exchange2.ExSharePersonCollection espc = exLandContractor.SharePersons;
            if (espc == null || espc.Count == 0)
            {
                return sharePersons;
            }
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
            return sharePersons;
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
        /// 地块映射
        /// </summary>
        /// <param name="exContractLand"></param>
        /// <returns></returns>
        private ContractLand ArcContractLandMapping(YuLinTu.Business.ContractLand.Exchange2.ExContractLand exContractLand)
        {
            ContractLand land = new ContractLand();
            land.ID = exContractLand.ID;
            land.ConcordId = (exContractLand.ConcordId == null || !exContractLand.ConcordId.HasValue) ? land.ConcordId : exContractLand.ConcordId.Value;
            land.ActualArea = exContractLand.ActualArea == 0.0 ? land.ActualArea : ToolMath.RoundNumericFormat(exContractLand.ActualArea * 0.0015, 2);
            land.AwareArea = exContractLand.AwareArea == 0.0 ? land.AwareArea : exContractLand.AwareArea;
            string fixNumber = string.Format("{0:D19}", 0);
            land.CadastralNumber = string.IsNullOrEmpty(exContractLand.CadastralNumber) ? fixNumber + exContractLand.AgricultureNumber : exContractLand.CadastralNumber;
            land.Comment = string.IsNullOrEmpty(exContractLand.Comment) ? land.Comment : exContractLand.Comment;
            land.ConstructMode = exContractLand.ContractType <= 0 ? land.ConstructMode : ((int)Enum.Parse(typeof(eContracterType), exContractLand.ContractType.ToString())).ToString();
            land.CreationTime = exContractLand.CreationTime == null ? land.CreationTime : exContractLand.CreationTime;
            land.Founder = string.IsNullOrEmpty(exContractLand.Founder) ? land.Founder : exContractLand.Founder;
            land.OwnerName = string.IsNullOrEmpty(exContractLand.HouseHolderName) ? land.OwnerName : exContractLand.HouseHolderName;
            land.OwnerId = (Contractor != null && Contractor.Name == land.OwnerName) ? Contractor.ID : Guid.Empty;
            land.IsFarmerLand = (exContractLand.IsFarmerLand != null && exContractLand.IsFarmerLand.HasValue) ? exContractLand.IsFarmerLand.Value : land.IsFarmerLand;
            land.IsFlyLand = exContractLand.IsFlyLand;
            land.LandCode = string.IsNullOrEmpty(exContractLand.LandCode) ? land.LandCode : exContractLand.LandCode;
            land.LandLevel = exContractLand.LandLevel <= 0 ? land.LandLevel : ((int)Enum.Parse(typeof(eContractLandLevel), exContractLand.LandLevel.ToString())).ToString();
            if (!string.IsNullOrEmpty(exContractLand.LandNeighbor))
            {
                string[] neighbor = exContractLand.LandNeighbor.Split('#');
                land.NeighborEast = neighbor[0];
                land.NeighborSouth = neighbor[1];
                land.NeighborWest = neighbor[2];
                land.NeighborNorth = neighbor[3];
            }
            land.LandNumber = string.IsNullOrEmpty(exContractLand.LandNumber) ? land.LandNumber : exContractLand.LandNumber;
            land.LandScopeLevel = exContractLand.LandScopeLevel <= 0 ? land.LandScopeLevel : ((int)Enum.Parse(typeof(eLandSlopeLevel), exContractLand.LandScopeLevel.ToString())).ToString();
            land.LineArea = exContractLand.LineArea == 0.0 ? land.LineArea : exContractLand.LineArea;
            land.ManagementType = exContractLand.ManagementType <= 0 ? land.ManagementType : ((int)Enum.Parse(typeof(eManageType), exContractLand.ManagementType.ToString())).ToString();
            land.ModifiedTime = exContractLand.ModifiedTime == null ? land.ModifiedTime : exContractLand.ModifiedTime;
            land.Modifier = string.IsNullOrEmpty(exContractLand.Modifier) ? land.Modifier : exContractLand.Modifier;
            land.Name = string.IsNullOrEmpty(exContractLand.Name) ? land.Name : exContractLand.Name;
            land.SenderName = string.IsNullOrEmpty(exContractLand.OwnerRightName) ? land.SenderName : exContractLand.OwnerRightName;
            land.OwnRightType = exContractLand.OwnerRightType <= 0 ? land.OwnRightType : ((int)Enum.Parse(typeof(eLandPropertyType), exContractLand.OwnerRightType.ToString())).ToString();
            land.PlantType = exContractLand.PlantType <= 0 ? land.PlantType : ((int)Enum.Parse(typeof(ePlantProtectType), exContractLand.PlantType.ToString())).ToString();
            land.PlotNumber = string.IsNullOrEmpty(exContractLand.PlotNumber) ? land.PlotNumber : exContractLand.PlotNumber;
            land.Purpose = exContractLand.Purpose <= 0 ? land.Purpose : ((int)Enum.Parse(typeof(eLandPurposeType), exContractLand.Purpose.ToString())).ToString();
            land.Soiltype = string.IsNullOrEmpty(exContractLand.Soiltype) ? land.Soiltype : exContractLand.Soiltype;
            land.TableArea = (exContractLand.TableArea == null || !exContractLand.TableArea.HasValue) ? land.TableArea : exContractLand.TableArea.Value;
            if (string.IsNullOrEmpty(land.LandNumber))
            {
                land.LandNumber = ContractLand.GetLandNumber(land.CadastralNumber);
            }
            land.Shape = exContractLand.Shape != null ? YuLinTu.Spatial.Geometry.FromBytes(exContractLand.Shape.WellKnownBinary, 0) : null;
            return land;
        }

        /// <summary>
        /// 地域映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <returns></returns>
        private Zone ArcZoneMapping(YuLinTu.Business.ContractLand.Exchange2.ExZone exZone)
        {
            Zone zone = new Zone();
            zone.ID = exZone.ID;
            zone.Code = exZone.Code;
            zone.Comment = exZone.Comment;
            zone.CreateTime = exZone.CreateTime;
            zone.CreateUser = exZone.CreateUser;
            zone.FullCode = exZone.FullCode;
            zone.FullName = exZone.FullName;
            zone.Level = (eZoneLevel)Enum.Parse(typeof(eZoneLevel), exZone.Level.ToString());
            zone.LastModifyTime = exZone.ModifyTime;
            zone.LastModifyUser = exZone.ModifyUser;
            zone.Name = exZone.Name;
            zone.UpLevelCode = exZone.ParentCode;
            zone.UpLevelName = exZone.ParentName;
            if (exZone.Shape != null)
                zone.Shape = YuLinTu.Spatial.Geometry.FromBytes(exZone.Shape.WellKnownBinary, 0);
            return zone;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            Contractor = null;
            Concord = null;
            ZoneCollection = null;
            LandCollection = null;
            GC.Collect();
        }

        #endregion

        #region Static

        /// <summary>
        /// 初始化服务器数据
        /// </summary>
        /// <returns></returns>
        public static YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient InitazlieServerData()
        {
            YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient landService = null;
            //UserEntity entity = ToolUser.DeserializeUser();
            //if (TheSecurity.Current.IsEncrypt)
            //{
            //    var binding = new System.ServiceModel.WSHttpBinding();
            //    binding.Security.Mode = System.ServiceModel.SecurityMode.Message;
            //    binding.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.UserName;
            //    binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
            //    binding.SendTimeout = TimeSpan.FromMinutes(30.0);
            //    binding.MaxBufferPoolSize = 104857600;
            //    binding.MaxReceivedMessageSize = 104857600;
            //    binding.MaxBufferPoolSize = 104857600;
            //    binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            //    binding.ReaderQuotas.MaxDepth = 32;
            //    binding.ReaderQuotas.MaxStringContentLength = 104857600;
            //    binding.ReaderQuotas.MaxArrayLength = 104857600;
            //    binding.ReaderQuotas.MaxBytesPerRead = 4096;
            //    binding.ReaderQuotas.MaxNameTableCharCount = 16384;

            //    var uri = new Uri(TheSecurity.Current.BusinessAccessDataServiceSite);
            //    var identity = System.ServiceModel.EndpointIdentity.CreateDnsIdentity("PRServer");
            //    var addr = new System.ServiceModel.EndpointAddress(uri, identity);

            //    landService = new YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient(binding, addr);
            //    var clientCredentials = landService.Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
            //    clientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            //    clientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = new YuLinTu.PropertyRight.Services.CustomX509Validator();
            //    landService.ClientCredentials.UserName.UserName = entity.UserName;
            //    string sessionCode = InitalizeLoginInformation(entity.UserName, entity.Password);
            //    landService.ClientCredentials.UserName.Password = sessionCode;
            //}
            //else
            //{
            //    var binding = new System.ServiceModel.BasicHttpBinding();
            //    binding.SendTimeout = TimeSpan.FromMinutes(30.0);
            //    binding.MaxBufferPoolSize = 104857600;
            //    binding.MaxReceivedMessageSize = 104857600;
            //    binding.MaxBufferPoolSize = 104857600;
            //    binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            //    binding.ReaderQuotas.MaxDepth = 32;
            //    binding.ReaderQuotas.MaxStringContentLength = 104857600;
            //    binding.ReaderQuotas.MaxArrayLength = 104857600;
            //    binding.ReaderQuotas.MaxBytesPerRead = 4096;
            //    binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            //    var addr = new System.ServiceModel.EndpointAddress(TheSecurity.Current.BusinessAccessDataServiceSite);
            //    landService = new YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient(binding, addr);
            //    landService.ClientCredentials.UserName.UserName = entity.UserName;
            //    string sessionCode = InitalizeLoginInformation(entity.UserName, entity.Password);
            //    landService.ClientCredentials.UserName.Password = sessionCode;
            //}
            //var serializerBehaviorType = typeof(BasicHttpBinding).Assembly.GetTypes().First(t => t.Name == "DataContractSerializerServiceBehavior");
            //var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance;
            //var serializerBehavior = serializerBehaviorType.GetConstructor(flags, null, new Type[] { typeof(bool), typeof(int) }, null)
            //    .Invoke(new object[] { false, 2147483647 });
            //landService.Endpoint.Behaviors.Add(serializerBehavior as IEndpointBehavior);
            return landService;
        }

        /// <summary>
        /// 初始化登录信息
        /// </summary>
        /// <returns></returns>
        public static string InitalizeLoginInformation(string userName, string password)
        {
            //try
            //{
            //    string secruityUrl = TheSecurity.Current.BusinessDataServiceSite;
            //    if (string.IsNullOrEmpty(secruityUrl))
            //    {
            //        return string.Empty;
            //    }
            //    secruityUrl = secruityUrl.Replace("amp;", "");
            //    secruityUrl += userName;
            //    secruityUrl += "&pwd=";
            //    secruityUrl += EncryptDES(password);
            //    secruityUrl = secruityUrl.Replace("+", "%2b");
            //    WebRequest tokenRequest = System.Net.WebRequest.Create(secruityUrl);
            //    tokenRequest.ContentType = "application/x-www-form-urlencoded";
            //    WebResponse tokenResponse = tokenRequest.GetResponse();
            //    Stream responseStream = tokenResponse.GetResponseStream();
            //    var readStream = new StreamReader(responseStream);
            //    string token = readStream.ReadToEnd();
            //    readStream.Close();
            //    if (string.IsNullOrEmpty(token))
            //    {
            //        return token;
            //    }
            //    token = token.Replace("\"", "");
            //    Guid id = new Guid(token);
            //    if (id != null && id != Guid.Empty)
            //    {
            //        return token;
            //    }
            //}
            //catch (SystemException ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.ToString());
            //}
            return string.Empty;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static string EncryptDES(string content, string container = "_default_")
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(container.Substring(0, 8));

            byte[] buffer;
            var desCSP = new DESCryptoServiceProvider();
            byte[] desKey = { };
            byte[] desIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, desCSP.CreateEncryptor(key, desIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cryStream);
            sw.WriteLine(content);
            sw.Close();
            cryStream.Close();
            buffer = ms.ToArray();

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="pToDecrypt">加密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的字符串</returns>
        private static string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));
        }

        /// <summary>
        /// 初始化登录用户最大根据地域
        /// </summary>
        /// <returns></returns>
        public static string InitalizeLoginRootZone()
        {
            return "";
            //try
            //{
            //    UserEntity entity = ToolUser.DeserializeUser();
            //    if (entity == null || entity.UserName.ToLower() == "guest")
            //    {
            //        return "";
            //    }
            //    string secruityUrl = TheSecurity.Current.BusinessDataServiceSite;
            //    if (string.IsNullOrEmpty(secruityUrl))
            //    {
            //        return string.Empty;
            //    }
            //    secruityUrl = secruityUrl.Replace("amp;", "");
            //    secruityUrl += entity.UserName;
            //    secruityUrl += "&pwd=";
            //    secruityUrl += EncryptDES(entity.Password);
            //    secruityUrl += "&isGetRoot=true";
            //    secruityUrl = secruityUrl.Replace("+", "%2b");
            //    WebRequest tokenRequest = System.Net.WebRequest.Create(secruityUrl);
            //    tokenRequest.ContentType = "application/x-www-form-urlencoded";
            //    WebResponse tokenResponse = tokenRequest.GetResponse();
            //    Stream responseStream = tokenResponse.GetResponseStream();
            //    var readStream = new StreamReader(responseStream);
            //    string token = readStream.ReadToEnd();
            //    readStream.Close();
            //    if (string.IsNullOrEmpty(token))
            //    {
            //        return token;
            //    }
            //    token = token.Replace("\"", "");
            //    return token;
            //}
            //catch (SystemException ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.ToString());
            //}
            //return string.Empty;
        }

        #endregion

        #region Exchange

        /// <summary>
        /// 转换交换实体
        /// </summary>
        public static YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection TransferExchangeEntity(YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection landColletion, string zoneCode)
        {
            if (landColletion == null || landColletion.Count == 0)
            {
                return null;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection familyCollection = new YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection();
            foreach (YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor in landColletion)
            {
                if (zoneCode != "#" && zoneCode != "86" && landContractor.Zone != null
                    && landContractor.Zone.FullCode.IndexOf(zoneCode) != 0)
                {
                    continue;
                }
                if (landContractor.ContractConcord != null)
                {
                    landContractor.ContractConcord.CountActualArea = ToolMath.RoundNumericFormat(landContractor.ContractConcord.CountActualArea * 0.0015, 2);
                }
                if (landContractor.ContractLands != null)
                {
                    foreach (YuLinTu.Business.ContractLand.Exchange2.ExContractLand land in landContractor.ContractLands)
                    {
                        land.ActualArea = ToolMath.RoundNumericFormat(land.ActualArea * 0.0015, 2);
                    }
                }
                if (landContractor.SharePersons != null)
                {
                    foreach (YuLinTu.Business.ContractLand.Exchange2.ExSharePerson person in landContractor.SharePersons)
                    {
                        person.Comment = LandCategoryMapping.SharePersonCommentCodeMapping(person.Comment);
                        person.Relation = YuLinTu.Library.Entity.RelationShipMapping.CodeMapping(person.Relation);
                        person.Relation = person.Relation == "其他" ? "" : person.Relation;
                    }
                }
                familyCollection.Add(landContractor);
            }
            landColletion = null;
            GC.Collect();
            return familyCollection;
        }

        #endregion

        #region Transfer

        /// <summary>
        /// 转换交换实体
        /// </summary>
        public static YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection TransferExchangeEntry(List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> bookCollection)
        {
            if (bookCollection == null || bookCollection.Count == 0)
            {
                return null;
            }
            bookCollection = AgricultureBookFilter(bookCollection);
            YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection familyCollection = new YuLinTu.Business.ContractLand.Exchange2.ExLandContractorCollection();
            foreach (YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book in bookCollection)
            {
                if (string.IsNullOrEmpty(book.SZDY))
                {
                    continue;
                }
                YuLinTu.Business.ContractLand.Exchange2.ExLandContractor family = new YuLinTu.Business.ContractLand.Exchange2.ExLandContractor();
                YuLinTu.Business.ContractLand.Exchange2.ExZone exZone = new YuLinTu.Business.ContractLand.Exchange2.ExZone();
                exZone.FullCode = (book.DJCBD != null && !string.IsNullOrEmpty(book.DJCBD[0].ZLDWDM)) ? book.DJCBD[0].ZLDWDM : book.SZDY;
                exZone.FullName = (book.DJCBD != null && !string.IsNullOrEmpty(book.DJCBD[0].ZLDWMC)) ? book.DJCBD[0].ZLDWMC : book.CBFZZ;
                family.Zone = exZone;
                family.Contractor = AgriContractorMapping(book);
                family.SharePersons = AgriSharePersonMapping(book);
                family.ContractLands = AgriLandMapping(book);
                AgriOtherMapping(family, book);
                familyCollection.Add(family);
            }
            bookCollection = null;
            GC.Collect();
            return familyCollection;
        }

        /// <summary>
        /// 农村土地承包经营权数据过滤
        /// </summary>
        /// <param name="bookCollection"></param>
        /// <returns></returns>
        private static List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> AgricultureBookFilter(List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> bookCollection)
        {
            if (bookCollection == null || bookCollection.Count == 0)
            {
                return bookCollection;
            }
            List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> survey = bookCollection.FindAll(bk => string.IsNullOrEmpty(bk.HTBH));
            List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> register = bookCollection.FindAll(bk => !string.IsNullOrEmpty(bk.HTBH));
            List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB> books = new List<PropertyRight.ContractLand.EX_CBJYQ_DJB>();
            if (register == null || register.Count == 0)
            {
                books = survey.Clone() as List<YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB>;
                survey = null;
                bookCollection = null;
                return books;
            }
            foreach (YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book in survey)
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

        /// <summary>
        /// 承包方信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private static YuLinTu.Business.ContractLand.Exchange2.ExContractor AgriContractorMapping(YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book)
        {
            if (book == null)
            {
                return null;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExContractor contractor = new YuLinTu.Business.ContractLand.Exchange2.ExContractor();
            contractor.ID = book.CBFID;
            contractor.Comment = book.CBFBZ;
            contractor.Name = book.CBFMC;
            contractor.Number = book.CBFZJHM;
            contractor.VirtualType = 1;
            contractor.Telephone = book.CBFLXDH;
            contractor.FamilyNumber = book.CBFBH;
            contractor.PostNumber = book.CBFYB;
            return contractor;
        }

        /// <summary>
        /// 共有人信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private static YuLinTu.Business.ContractLand.Exchange2.ExSharePersonCollection AgriSharePersonMapping(YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book)
        {
            if (book.DJGYR == null || book.DJGYR.Count == 0)
            {
                return null;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExSharePersonCollection personCollection = new YuLinTu.Business.ContractLand.Exchange2.ExSharePersonCollection();
            foreach (YuLinTu.PropertyRight.ContractLand.DJ_CBJYQ_JTCBGYR person in book.DJGYR)
            {
                YuLinTu.Business.ContractLand.Exchange2.ExSharePerson sharePerson = new YuLinTu.Business.ContractLand.Exchange2.ExSharePerson();
                sharePerson.ID = person.ID;
                sharePerson.Age = person.GetAge();
                sharePerson.BirthDate = (person.CSRQ != null && person.CSRQ.HasValue && person.CSRQ.Value.Year == 0001) ? null : person.CSRQ;
                sharePerson.Comment = LandCategoryMapping.SharePersonCommentCodeMapping(person.BZ);
                if (!string.IsNullOrEmpty(person.Comment))
                {
                    string[] val = person.Comment.Split(new char[] { '|' });
                    if (val != null && val.Length > 0)
                    {
                        object obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eLandCategoryType), val[0]);
                        sharePerson.Nationality = obj == null ? EnumNameAttribute.GetDescription(eNation.UnKnown) : EnumNameAttribute.GetDescription((eNation)(obj));
                    }
                    sharePerson.Comment = (val != null && val.Length > 1) ? val[1] : "";
                }
                if (string.IsNullOrEmpty(sharePerson.Comment))
                {
                    sharePerson.Comment = person.BZSM;
                }
                sharePerson.ContractorId = book.CBFID;
                sharePerson.CredentialNumber = person.ZJHM;
                sharePerson.CredentialType = (int)eCredentialsType.IdentifyCard;
                sharePerson.Name = person.XM;
                sharePerson.Relation = YuLinTu.Library.Entity.RelationShipMapping.CodeMapping(person.YHZGX);
                sharePerson.Relation = sharePerson.Relation == "其他" ? "" : sharePerson.Relation;
                sharePerson.Sex = Convert.ToInt32(person.XB);
                personCollection.Add(sharePerson);
            }
            return personCollection;
        }

        /// <summary>
        /// 地块信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private static YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection AgriLandMapping(YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book)
        {
            if (book.DJCBD == null || book.DJCBD.Count == 0)
            {
                return null;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection lands = new YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection();
            foreach (YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_CBD land in book.DJCBD)
            {
                YuLinTu.Business.ContractLand.Exchange2.ExContractLand exLand = new YuLinTu.Business.ContractLand.Exchange2.ExContractLand();
                exLand.ActualArea = ToolMath.RoundNumericFormat(land.SCMJ * 0.0015, 2);
                exLand.AwareArea = (land.QQMJ != null && land.QQMJ.HasValue) ? land.QQMJ.Value : 0.0;
                exLand.CadastralNumber = InitalizeLandNumber(book, land.DKBM);
                exLand.Comment = land.DKBZXX;
                exLand.ConcordId = book.HTID;
                exLand.ContractType = (int)LandCategoryMapping.LandCategoryCodeMapping(land.DKLB);
                exLand.FormerPerson = book.CBFMC;
                exLand.HouseHolderName = book.CBFMC;
                exLand.ID = land.ID;
                exLand.IsFarmerLand = (land.SFJBNT != null && land.SFJBNT.HasValue) ? land.SFJBNT.Value : true;
                exLand.IsFlyLand = land.SFFD;
                exLand.LandCode = land.DL;
                exLand.LandLevel = Convert.ToInt32(land.DJ);
                exLand.LandName = land.DLBM;
                exLand.LandNeighbor = InitalizeLandNeighbor(land.SZ);
                exLand.LandNeighbor = land.SZ.Replace("#", "\r");
                exLand.Name = land.DKMC;
                exLand.Purpose = Convert.ToInt32(land.TDYT);
                exLand.TableArea = land.ELHTMJ;
                lands.Add(exLand);
            }
            return lands;
        }

        /// <summary>
        /// 初始化地块编码
        /// </summary>
        /// <param name="book"></param>
        /// <param name="landNumber"></param>
        /// <returns></returns>
        private static string InitalizeLandNumber(YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB book, string landNumber)
        {
            string number = "";
            if (book.SZDY.Length == Zone.ZONE_VILLAGE_LENGTH)
            {
                number = book.SZDY + "0000000";
            }
            else if (book.SZDY.Length == Zone.ZONE_GROUP_LENGTH)
            {
                number = book.SZDY + "000";
            }
            else
            {
            }
            return number + landNumber;
        }

        /// <summary>
        /// 初始化四至信息
        /// </summary>
        /// <param name="landNeighbor"></param>
        /// <returns></returns>
        private static string InitalizeLandNeighbor(string landNeighbor)
        {
            if (landNeighbor == null || landNeighbor.Length < 4)
            {
                return "####";
            }
            string[] neighbors = landNeighbor.Split(new char[] { '\r' });
            if (neighbors == null || neighbors.Length != 4)
            {
                neighbors = landNeighbor.Split(new char[] { '\n' });//xml中会变成\n
            }
            if (neighbors == null || neighbors.Length != 4)
            {
                neighbors = landNeighbor.Split(new char[] { '#' });//xml中会变成\n
            }
            if (neighbors == null || neighbors.Length != 4)
            {
                return "####";
            }
            return landNeighbor;
        }

        /// <summary>
        /// 地域信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private static void AgriOtherMapping(YuLinTu.Business.ContractLand.Exchange2.ExLandContractor family, YuLinTu.PropertyRight.ContractLand.EX_CBJYQ_DJB djb)
        {
            try
            {
                if (djb == null)
                {
                    return;
                }
                YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exConcord = new YuLinTu.Business.ContractLand.Exchange2.ExContractConcord();
                YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook exBook = new YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook();
                exConcord.ID = djb.HTID;
                exConcord.ConcordNumber = djb.HTBH;
                exConcord.ArableLandEndTime = djb.CBJSRQ;
                exConcord.ArableLandStartTime = djb.CBKSRQ;
                exConcord.ArableLandType = (int)LandCategoryMapping.LandModeCodeMapping(djb.CBFS);
                exConcord.ContracerType = 1;
                exConcord.ContractDate = djb.CBHTQDRQ;
                exConcord.ContractorId = djb.CBFID;
                exConcord.ContracterName = djb.CBFMC;
                exConcord.CountActualArea = ToolMath.RoundNumericFormat(djb.CBDSCZMJ * 0.0015, 2);
                exConcord.CountAwareArea = (djb.QQZMJ != null && djb.QQZMJ.HasValue) ? djb.QQZMJ.Value : 0.0;
                exConcord.ID = djb.HTID;
                exConcord.IsValid = false;
                exConcord.LandPurpose = Convert.ToInt32(djb.TDCBYT);
                exConcord.SenderName = djb.FBF != null ? djb.FBF.MC : djb.FBFQC;
                exConcord.EmployerId = djb.FBF != null ? djb.FBF.ID : Guid.Empty;
                exConcord.TotalTableArea = (djb.CBDELHTZMJ != null && djb.CBDELHTZMJ.HasValue) ? djb.CBDELHTZMJ.Value : 0.0;
                family.ContractConcord = exConcord;
                exBook.ID = djb.ID;
                exBook.Comment = djb.BZ;
                exBook.Number = djb.QZBH;
                exBook.RegeditNumber = djb.QZBH;
                if (djb.QZ != null)
                {
                    exBook.PrintDate = djb.QZ.DZRQ;
                    exBook.SendDate = djb.QZ.FZRQ;
                    exBook.SendOrganization = djb.QZ.FZJGJC;
                    exBook.WriteDate = djb.QZ.CJSJ;
                    exBook.Count = djb.QZ.DYCS;
                    exBook.Year = djb.QZ.DZRQ.Year.ToString();
                }
                family.ContractRegeditBook = exBook;
                YuLinTu.Business.ContractLand.Exchange2.ExCollectiveTissue tissue = new YuLinTu.Business.ContractLand.Exchange2.ExCollectiveTissue();
                tissue.Name = djb.FBFQC;
                tissue.Code = djb.FBFBM;
                if (djb.FBF != null)
                {
                    tissue.ID = djb.FBF.ID;
                    tissue.ZoneCode = djb.SZDY;
                }
                family.Contractee = tissue;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
