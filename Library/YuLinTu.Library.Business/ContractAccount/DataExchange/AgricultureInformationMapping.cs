/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using YuLinTu.PropertyRight.ContractLand;
using YuLinTu.Library.Business;
using YuLinTu.PropertyRight.Services.Client;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包地块信息映射
    /// </summary>
    public class AgricultureInformationMapping
    {
        #region Fields

        private int landIndex = 1;//地块索引
        private string recordInformation;//记录信息
        private string errorInformation;//错误信息

        #endregion Fields

        #region Delegate

        public delegate void InformationChangedEventHandler(string information);

        public event InformationChangedEventHandler InformationReportged;

        public delegate void ErrorInformationEventHandler(string information);

        public event ErrorInformationEventHandler ErrorReportged;

        #endregion Delegate

        #region Propertys

        /// <summary>
        /// 数据实例
        /// </summary>
        public IDbContext DataInstance { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnLine { get; set; }

        /// <summary>
        /// 是否是户编码
        /// </summary>
        public bool IsFamilyCoding { get; set; }

        /// <summary>
        /// 是否标准编码
        /// </summary>
        public bool IsStandCode { get; set; }

        /// <summary>
        /// 地块服务
        /// </summary>
        public ContractLandRegistrationServiceClient LandService { get; set; }

        /// <summary>
        /// 农用地实体
        /// </summary>
        public AgriLandEntity ArgicultureEntity { get; set; }

        /// <summary>
        /// 发包方集合
        /// </summary>
        public List<CollectivityTissue> TissueCollection { get; set; }

        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool HasError
        {
            get
            {
                return !string.IsNullOrEmpty(errorInformation);
            }
        }

        /// <summary>
        /// 台账常规设置
        /// </summary>
        public ContractBusinessSettingDefine ContractBusinessSettingDefine { get; set; }

        /// <summary>
        /// 操作类型,导入压缩包 1，还是上传  0
        /// </summary>
        public int OperateType { get; set; }

        #endregion Propertys

        #region Ctor

        public AgricultureInformationMapping()
        {
        }

        #endregion Ctor

        #region 数据映射

        /// <summary>
        /// 初始化分户户信息
        /// </summary>
        /// <param name="vp"></param>
        /// <returns></returns>
        public SplitFamilyRegisterItem InitalizeFamilyregisterItem(EX_CBJYQ_DJB djb, VirtualPerson vp)
        {
            List<ContractConcord> concords = DataInstance.CreateConcordStation().GetAllConcordByFamilyID(vp.ID);
            ContractConcord concord = (concords != null && concords.Count > 0) ? concords[0] : null;
            YuLinTu.PropertyRight.SerialNumber sn = null;
            if (IsOnLine && LandService != null)
            {
                sn = LandService.GenerateSerialNumbers(vp.ZoneCode, PropertyRight.eSerialNumberType.Warrant);
            }
            SplitFamilyRegisterItem item = new SplitFamilyRegisterItem();
            item.XHDJBID = djb.ID;
            item.QZBH = (djb != null && !string.IsNullOrEmpty(djb.QZBH)) ? djb.QZBH : (IsOnLine ? sn.FormatCode() : InitalizeWarrantNumber(vp));
            if (IsOnLine && LandService != null)
            {
                sn = LandService.GenerateSerialNumbers(vp.ZoneCode, PropertyRight.eSerialNumberType.Obligee);
            }
            item.CBFBH = (djb != null && !string.IsNullOrEmpty(djb.CBFBH)) ? djb.CBFBH : (IsOnLine ? sn.FormatCode() : InitalizeFamilyNumber(vp.ZoneCode, vp.FamilyNumber));
            item.CBFZJHM = string.IsNullOrEmpty(vp.Number) ? "" : vp.Number.ToUpper();
            item.XHHZMC = vp.Name;
            item.CBFZZ = vp.Address;
            item.CBFYB = vp.PostalNumber;
            item.CBFLXDH = vp.Telephone;
            int cardType = (int)vp.CardType;
            item.CBFZJLX = cardType == 0 ? ((int)eCredentialsType.IdentifyCard).ToString() : cardType.ToString();
            item.HTBH = (djb != null && !string.IsNullOrEmpty(djb.HTBH)) ? djb.HTBH : (IsOnLine ? sn.FormatCode() : InitalizeWarrantNumber(vp));
            item.CBFS = concord != null ? concord.ArableLandType : "110";
            item.CBKSRQ = (concord != null && concord.ArableLandStartTime != null && concord.ArableLandStartTime.HasValue) ? concord.ArableLandStartTime.Value : DateTime.Now;
            item.CBJSRQ = (concord != null && concord.ArableLandEndTime != null && concord.ArableLandEndTime.HasValue) ? concord.ArableLandEndTime.Value : DateTime.Now;
            object obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eLandPurposeType), concord.LandPurpose.ToString());
            item.TDCBYT = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? ((int)YuLinTu.Library.Entity.eLandPurposeType.Planting).ToString() : concord.LandPurpose;
            item.DJRQ = DateTime.Now;
            item.BZ = concord != null ? concord.Comment : "";
            List<Person> personList = vp.SharePersonList;
            SplitFamilyMemberRegsiterItem[] regItem = new SplitFamilyMemberRegsiterItem[personList.Count];
            for (int i = 0; i < personList.Count; i++)
            {
                Person person = personList[i] as Person;
                SplitFamilyMemberRegsiterItem sharePerson = new SplitFamilyMemberRegsiterItem();
                sharePerson.MC = person.Name;
                sharePerson.SFZH = person.ICN;
                regItem[i] = sharePerson;
            }
            item.GYR = regItem;
            List<ContractLand> landCollection = DataInstance.CreateContractLandWorkstation().GetCollection(vp.ID);
            Guid[] ids = new Guid[landCollection.Count];
            for (int i = 0; i < landCollection.Count; i++)
            {
                ids[i] = landCollection[i].ID;
            }
            item.CBD = ids.Clone() as Guid[];
            concord = null;
            landCollection = null;
            ids = null;
            item.DJSQS = InitalizetionRequireBook(djb, "");
            return item;
        }

        /// <summary>
        ///  生成登记簿
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public EX_CBJYQ_DJB InitalizeRegisterBook(Zone zone, VirtualPerson vp, ContractConcord concord, PropertyRight.Registration.eApplicationType appType = PropertyRight.Registration.eApplicationType.InitializationRegister)
        {
            if (vp == null)
            {
                return null;
            }
            try
            {
                if (concord != null && string.IsNullOrEmpty(concord.ConcordNumber))
                {
                    concord = null;
                }
                EX_CBJYQ_DJB djb = new EX_CBJYQ_DJB();
                djb.ID = concord == null ? Guid.NewGuid() : concord.ID;
                AgriContractorMapping(djb, vp);//承包方信息
                AgriSharePersonMapping(djb, vp);//共有人信息
                AgricultureLandMapping(djb, zone, vp, concord);//承包地块信息
                djb.YWZT = YuLinTu.PropertyRight.Registration.eRegisterStatus.BeginReceive;
                AgricultureSenderMapping(djb, concord);//集体经济组织
                djb.DJSQS = InializeRequireBookInformation(djb, appType, "");
                if (concord == null)
                {
                    return djb;
                }
                AgricultureConcordMapping(djb, vp, concord);//合同信息
                AgricultureRequirBookMapping(djb, concord);//申请书
                AgricultureBookMapping(djb, concord);//权证
                if (djb.HTBH == djb.YHTBH)// 如果合同编号与原合同编号一致，则表明没有原合同编号。
                {
                    djb.YHTBH = null;
                }
                if (djb.QZBH == djb.YQZBH)// 如果权证编号与原权证编号一致，则表明没有原权证编号。
                {
                    djb.YQZBH = null;
                }
                return djb;
            }
            catch (Exception ess)
            {
                var ds = ess.Message;
                return null;
            }
        }

        /// <summary>
        ///  生成登记簿
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public EX_CBJYQ_DJB InitalizeUnRegisterBook(Zone zone, VirtualPerson vp, List<ContractConcord> concords, PropertyRight.Registration.eApplicationType appType = PropertyRight.Registration.eApplicationType.InitializationRegister)
        {
            if (vp == null)
            {
                return null;
            }
            EX_CBJYQ_DJB djb = new EX_CBJYQ_DJB();
            djb.ID = Guid.NewGuid();
            AgricultureUnLandMapping(djb, zone, vp, concords);//承包地块信息
            AgriContractorMapping(djb, vp);//承包方信息
            AgriSharePersonMapping(djb, vp);//共有人信息
            AgricultureUnConcordMapping(djb, null);//合同信息
            djb.YWZT = YuLinTu.PropertyRight.Registration.eRegisterStatus.BeginReceive;
            AgricultureSenderMapping(djb, null);//集体经济组织
            djb.DJSQS = InializeRequireBookInformation(djb, appType, "");

            return djb;
        }

        /// <summary>
        /// 承包方信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgriContractorMapping(EX_CBJYQ_DJB regBook, VirtualPerson vp)
        {
            regBook.CBFID = vp.ID;
            regBook.CBFLXDH = vp.Telephone;
            regBook.CBFMC = vp.Name.Trim();
            regBook.CBFBH = InitalizeFamilyNumber(vp.ZoneCode, vp.FamilyNumber);
            regBook.CBFZJHM = string.IsNullOrEmpty(vp.Number) ? "" : vp.Number.ToUpper();
            regBook.CBFLX = ((int)vp.FamilyExpand.ContractorType).ToString();
            int cardType = (int)vp.CardType;
            regBook.CBFZJLX = cardType == 0 ? ((int)eCredentialsType.IdentifyCard).ToString() : cardType.ToString();
            regBook.CBFYB = vp.PostalNumber;
            regBook.CBFZZ = vp.Address;
            regBook.DJRQ = DateTime.Now;
            regBook.SZDY = vp.ZoneCode.Trim();
            regBook.CBFDCY = vp.FamilyExpand.SurveyPerson;
            regBook.CBFDCJS = vp.FamilyExpand.SurveyChronicle;
            if (vp.FamilyExpand.SurveyDate != null && vp.FamilyExpand.SurveyDate.HasValue)
            {
                regBook.CBFDCRQ = vp.FamilyExpand.SurveyDate.Value;
            }
            regBook.CBFDCSHYJ = vp.FamilyExpand.CheckOpinion;
            regBook.CBFDCSHR = vp.FamilyExpand.CheckPerson;
            regBook.CBFDCSHRQ = vp.FamilyExpand.CheckDate;
            regBook.CBFELCBFS = ((int)(vp.FamilyExpand.ConstructMode)).ToString();
            AgriContractorExpandMapping(regBook, vp);
        }

        /// <summary>
        ///  承包方扩展信息
        /// </summary>
        private void AgriContractorExpandMapping(EX_CBJYQ_DJB regBook, VirtualPerson vp)
        {
            try
            {
                regBook.CBFBZ = vp.Comment;
                VirtualPersonExpand expand = vp.FamilyExpand;
                if (!string.IsNullOrEmpty(expand.HouseHolderName))
                {
                    regBook.FamilyComment = vp.OtherInfomation;//所有信息都放在此处理
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 初始化户号
        /// </summary>
        /// <param name="familyNumber"></param>
        /// <returns></returns>
        private string InitalizeFamilyNumber(string codeZone, string familyNumber)
        {
            int number = 0;
            Int32.TryParse(familyNumber, out number);
            string famNumber = familyNumber;
            string zoneCode = codeZone;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_VILLAGE_LENGTH:
                    zoneCode = codeZone + "00";
                    break;
                //case Zone.ZONE_GROUP_LENGTH:
                //    zoneCode = codeZone.Substring(0, 12) + codeZone.Substring(14);
                //    break;
                default:
                    break;
            }
            famNumber = zoneCode + string.Format("{0:D4}", number);
            return famNumber;
        }

        /// <summary>
        /// 共有人信息映射
        /// </summary>
        private void AgriSharePersonMapping(EX_CBJYQ_DJB regBook, VirtualPerson constractor)
        {
            List<Person> personList = constractor.SharePersonList;
            List<DJ_CBJYQ_JTCBGYR> ret = new List<DJ_CBJYQ_JTCBGYR>(personList.Count);
            personList.ForEach(p =>
            {
                if (p.EntityStatus != eEntityStatus.Deleted)
                {
                    ret.Add(new DJ_CBJYQ_JTCBGYR
                    {
                        BZ = LandCategoryMapping.SharePersonCommentNameMapping(p.Comment),
                        Comment = ((int)p.Nation).ToString() + "|" + p.Comment,
                        CSRQ = SharePersonBirthdayProgress(p),
                        ID = p.ID,
                        XB = LandCategoryMapping.GenderNameMapping(p.Gender),
                        XM = p.Name,
                        YHZGX = string.IsNullOrEmpty(p.Relationship) ? "" : RelationShipMapping.NameMapping(ToolString.ExceptSpaceString(p.Relationship)),
                        Relationship = p.Relationship,
                        BZSM = p.Comment,
                        ZJHM = string.IsNullOrEmpty(p.ICN) ? "" : p.ICN.ToUpper(),
                        ZJLX = InitalizePersonCardType(p).ToString(),
                        SFGYR = string.IsNullOrEmpty(p.IsSharedLand) ? true : (p.IsSharedLand == "是" ? true : false),
                        DJBID = regBook.ID,
                    });
                }
            });
            regBook.DJGYR = ret;
            regBook.DJGYR.ForEach(d => d.DJBID = regBook.ID);
        }

        /// <summary>
        /// 初始化共有人证件类型
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private int InitalizePersonCardType(Person person)
        {
            int val = (int)person.CardType;
            if (val == 0)
            {
                val = (int)(eCredentialsType.IdentifyCard);
            }
            return val;
        }

        /// <summary>
        /// 共有人出生日期处理
        /// </summary>
        /// <param name="person"></param>
        private DateTime? SharePersonBirthdayProgress(Person person)
        {
            DateTime? birthDay = null;
            if (!string.IsNullOrEmpty(person.ICN) && person.CardType == eCredentialsType.IdentifyCard)
            {
                birthDay = ToolICN.GetBirthday(person.ICN);
                if (birthDay != null && birthDay.HasValue && birthDay.Value.Year < 1800)
                {
                    birthDay = null;
                }
                return birthDay;
            }
            if (person.Birthday == null)
            {
                return null;
            }
            if (!person.Birthday.HasValue)
            {
                return null;
            }
            if (person.Birthday.Value.Year < 1800)
            {
                return null;
            }
            if (person.Birthday.Value.Year == DateTime.Now.Year && person.Birthday.Value.Month == DateTime.Now.Month
                && person.Birthday.Value.Day == DateTime.Now.Day)
            {
                return null;
            }
            return person.Birthday;
        }

        /// <summary>
        ///  转换性别
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        private YuLinTu.PropertyRight.eGender TransformGender(int gender)
        {
            switch (gender)
            {
                case 0:
                    return YuLinTu.PropertyRight.eGender.Female;

                case 1:
                    return YuLinTu.PropertyRight.eGender.Male;

                default:
                    return YuLinTu.PropertyRight.eGender.Unknow;
            }
        }

        /// <summary>
        /// 地块信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgricultureLandMapping(EX_CBJYQ_DJB regBook, Zone zone, VirtualPerson constractor, ContractConcord concord)
        {
            try
            {
                List<ContractLand> landArray = InitalizeLandArray(constractor, concord);
                List<EX_CBJYQ_CBD> lands = new List<EX_CBJYQ_CBD>();
                SpatialReference reference = null;
                foreach (ContractLand land in landArray)
                {
                    if (land.Status == ((int)eEntityStatus.Deleted).ToString())/* || string.IsNullOrEmpty(land.CadastralNumber))*/
                    {
                        continue;
                    }

                    if (land.Shape == null)
                    {
                        reference = new SpatialReference() { WKID = 2380 };
                    }
                    else
                    {
                        reference = land.Shape.SpatialReference;
                    }
                    EX_CBJYQ_CBD exLand = SetAgriLandInformation(zone, land);
                    exLand.JZD = InitalizeDotInformation(regBook, land, reference);//界址点
                    exLand.JZX = InitalizeLineInformation(regBook, land, reference);//界址线
                    if (land.Shape != null)
                    {
                        byte[] wkb = land.Shape.ToBytes();
                        if (wkb != null && wkb.Length > 0)
                        {
                            exLand.Shape = new GeometryWellKnownValue { CoordinateSystemId = land.Shape.SpatialReference.WKID, WellKnownBinary = wkb };
                        }
                    }
                    ReportShapeErrorInformation(land);

                    if (exLand.SCMJ <= 0 || land.ActualArea <= 0.0)
                    {
                        string info = string.Format("承包方:{0}下地块编码为{1}的地块未填写实测面积!", land.OwnerName, land.LandNumber);
                        ReportErrorInformation(info);
                    }
                    lands.Add(exLand);
                    landIndex++;
                }
                regBook.DJCBD = lands;
                regBook.DJCBD.ForEach(d =>
                {
                    d.DJBID = regBook.ID;
                    if (concord != null)
                    {
                        d.SYQRID = concord.SenderId;
                    }
                });
                regBook.LYXZ = TransformResUtilizations(landArray);
                regBook.CBDELHTZMJ = landArray.Sum(l => (l != null && l.TableArea.HasValue) ? l.TableArea.Value : 0.0);//地块信息
                regBook.CBDKZS = landArray.Count;
                regBook.CBDSCZMJ = regBook.DJCBD.Sum(l => l != null ? l.SCMJ : 0.0);
                regBook.QQZMJ = landArray.Sum(l => l != null ? l.AwareArea : 0.0);
                landArray = null;
            }
            catch (Exception dd)
            {
                var dsafedsa = dd.Message;
            }
        }

        /// <summary>
        /// 地块信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgricultureUnLandMapping(EX_CBJYQ_DJB regBook, Zone zone, VirtualPerson constractor, List<ContractConcord> concords)
        {
            try
            {
                List<ContractLand> landArray = ArgicultureEntity.Lands.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == constractor.ID);
                if (landArray == null || landArray.Count == 0)
                {
                    return;
                }
                List<EX_CBJYQ_CBD> lands = new List<EX_CBJYQ_CBD>();
                SpatialReference reference = null;
                foreach (ContractLand land in landArray)
                {
                    if (land.Status == ((int)eEntityStatus.Deleted).ToString()) /*|| string.IsNullOrEmpty(land.CadastralNumber))*/
                    {
                        continue;
                    }
                    reference = (reference == null && land.Shape != null) ? land.Shape.SpatialReference : reference;
                    EX_CBJYQ_CBD exLand = SetAgriLandInformation(zone, land);
                    exLand.JZD = InitalizeDotInformation(regBook, land, reference);//界址点
                    exLand.JZX = InitalizeLineInformation(regBook, land, reference);//界址线
                    if (land.Shape != null)
                    {
                        byte[] wkb = land.Shape.ToBytes();
                        if (wkb != null && wkb.Length > 0)
                        {
                            exLand.Shape = new GeometryWellKnownValue { CoordinateSystemId = land.Shape.SpatialReference.WKID, WellKnownBinary = wkb };
                        }
                    }
                    ReportShapeErrorInformation(land);

                    if (OperateType == 0 && (land.LandNumber.IsNullOrEmpty() || land.LandNumber.Length != 19))
                    {
                        string info = string.Format("承包方:{0}下地块编码为{1}不符合农业部规范!", land.OwnerName, land.LandNumber);
                        ReportErrorInformation(info);
                    }
                    if (exLand.SCMJ <= 0.0 || land.ActualArea <= 0.0)
                    {
                        string info = string.Format("承包方:{0}下地块编码为{1}的地块未填写实测面积!", land.OwnerName, land.LandNumber);
                        ReportErrorInformation(info);
                    }
                    lands.Add(exLand);
                    landIndex++;
                }
                regBook.DJCBD = lands;
                regBook.DJCBD.ForEach(d =>
                {
                    d.DJBID = regBook.ID;
                });
                regBook.LYXZ = TransformResUtilizations(landArray);
                regBook.CBDELHTZMJ = landArray.Sum(l => (l != null && l.TableArea.HasValue) ? l.TableArea.Value : 0.0);//地块信息
                regBook.CBDKZS = landArray.Count;
                regBook.CBDSCZMJ = lands.Sum(l => l != null ? l.SCMJ : 0.0);
                regBook.QQZMJ = landArray.Sum(l => l != null ? l.AwareArea : 0.0);
                landArray = null;
            }
            catch (Exception reginfo)
            {
                var reginfoss = reginfo.Message;
            }
        }

        /// <summary>
        /// 报告错误信息
        /// </summary>
        private void ReportShapeErrorInformation(ContractLand land)
        {
            if (land == null)
            {
                string info = string.Format("承包方:{0}下地块编码为{1}的地块没有空间信息!", land.OwnerName, land.LandNumber);
                ReportInformation(info);
                return;
            }
            if (land.Shape == null)
            {
                string info = string.Format("承包方:{0}下地块编码为{1}的地块没有空间信息!", land.OwnerName, land.LandNumber);
                ReportInformation(info);
            }
            if (land.Shape != null && land.Shape.IsValid() == false)
            {
                string info = string.Format("承包方:{0}下地块编码为{1}的地块空间信息无效!", land.OwnerName, land.LandNumber);
                ReportInformation(info);
            }
        }

        /// <summary>
        /// 初始化地块集合
        /// </summary>
        /// <returns></returns>
        private List<ContractLand> InitalizeLandArray(VirtualPerson constractor, ContractConcord concord)
        {
            List<ContractLand> landArray = null;
            var landStation = DataInstance.CreateContractLandWorkstation();
            if (concord == null || string.IsNullOrEmpty(concord.ConcordNumber))
            {
                landArray = ArgicultureEntity.Lands.Count == 0 ? landStation.GetCollection(constractor.ID) : ArgicultureEntity.Lands.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == constractor.ID);
            }
            else
            {
                landArray = ArgicultureEntity.Lands.Count == 0 ? landStation.GetByConcordId(concord.ID) : ArgicultureEntity.Lands.FindAll(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == concord.ID);//找地通过合同id去找
            }
            return landArray;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        private EX_CBJYQ_CBD SetAgriLandInformation(Zone zone, ContractLand land)
        {
            EX_CBJYQ_CBD exLand = new EX_CBJYQ_CBD();
            try
            {
                exLand.CBFMC = land.OwnerName;
                string value = EnumNameAttribute.GetDescription(land.LandLevel);
                if (!string.IsNullOrEmpty(value) && ToolMath.MatchEntiretyNumber(value))
                {
                    exLand.DJ = land.LandLevel;
                }
                else if (string.IsNullOrEmpty(land.LandLevel))
                {
                    exLand.DJ = "900";
                }

                if (land.PlatType != null && land.PlatType != "")
                {
                    exLand.ZZLX = (YuLinTu.PropertyRight.ContractLand.ePlantingType)Enum.Parse(typeof(YuLinTu.PropertyRight.ContractLand.ePlantingType), land.PlatType);
                }
                else if (land.PlatType.IsNullOrEmpty())
                {
                    exLand.ZZLX = YuLinTu.PropertyRight.ContractLand.ePlantingType.Other;
                }

                exLand.DKMC = land.Name.IsNullOrEmpty() ? "" : land.Name.Replace("\0", "");
                //陈泽林 20161227 地块编码是什么就导出什么
                exLand.DKBM = land.LandNumber;

                exLand.YDKBM = (ContractLand.GetLandNumber(land.CadastralNumber) == exLand.DKBM) ? "" : ContractLand.GetLandNumber(land.CadastralNumber);
                exLand.ZLDWDM = land.IsFlyLand ? zone.UpLevelCode : land.ZoneCode;
                if (exLand.ZLDWDM.IsNullOrEmpty())
                {
                    exLand.ZLDWDM = zone.FullCode;
                    if (IsStandCode && zone.FullCode.Length == 14)
                    {
                        exLand.ZLDWDM = zone.FullCode;
                    }
                    else if (IsStandCode && zone.FullCode.Length == 16)
                    {
                        exLand.ZLDWDM = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14, 2);
                    }
                    else if (!IsStandCode && zone.FullCode.Length == 14)
                    {
                        exLand.ZLDWDM = zone.FullCode.Substring(0, 12) + "00" + zone.FullCode.Substring(12, 2);
                    }
                }
                if (ContractBusinessSettingDefine.ExportCompatibleOldDataExchange)
                {
                    if (zone.FullCode.Length == 14)
                    {
                        exLand.ZLDWDM = zone.FullCode.Substring(0, 12) + "00" + zone.FullCode.Substring(12, 2);
                    }
                    exLand.DKBM = land.LandNumber;
                    exLand.YDKBM = "";
                }
                exLand.TDYT = string.IsNullOrEmpty(land.Purpose) ? ((int)YuLinTu.Library.Entity.eLandPurposeType.Planting).ToString() : land.Purpose;
                exLand.DL = land.LandCode;
                exLand.ELHTMJ = land.TableArea;
                exLand.GTDJBH = land.ExtendB;
                exLand.ID = land.ID;
                exLand.SCMJ = land.ActualArea / 0.0015;
                exLand.SFJBNT = land.IsFarmerLand;
                exLand.SYQRID = zone.ID;
                exLand.YDKID = land.ID;
                if (string.IsNullOrEmpty(land.PlantType) || land.PlantType == "3" || land.PlantType == "UnKnown" || land.PlantType == "Unknown")
                {
                    exLand.GBLX = ePlantProtectionType.Unknown;
                }
                else
                {
                    if (land.PlantType == "1")
                        exLand.GBLX = ePlantProtectionType.FirstGrade;
                    if (land.PlantType == "2")
                        exLand.GBLX = ePlantProtectionType.SecondGrade;
                }
                exLand.SZDY = land.ZoneCode.IsNullOrEmpty() ? "" : land.ZoneCode;
                exLand.ZLDWMC = land.LocationName.IsNullOrEmpty() ? " " : land.LocationName; //land.ZoneName.IsNullOrEmpty() ? " " : land.ZoneName;
                exLand.QQMJ = land.AwareArea;
                if (land.ManagementType.IsNullOrEmpty())
                {
                    exLand.JYFS = eLandManagementType.Other;
                }
                else
                {
                    var jyfs = (eLandManagementType)Enum.Parse(typeof(eLandManagementType), land.ManagementType);
                    if (Enum.IsDefined(typeof(eLandManagementType), jyfs))
                        exLand.JYFS = jyfs;
                    else
                        exLand.JYFS = eLandManagementType.SelfSupport;
                }
                exLand.SFFD = land.IsFlyLand;
                exLand.SFQQQG = land.IsStockLand;
                exLand.JDDMJ = land.MotorizeLandArea;
                exLand.DLBM = land.LandName.IsNullOrEmpty() ? "" : land.LandName;
                exLand.DKLB = land.LandCategory.IsNullOrEmpty() ? "" : land.LandCategory;
                exLand.SYQXZ = land.OwnRightType.IsNullOrEmpty() ? "30" : land.OwnRightType;
                if (land.PlantType == ((int)ePlantProtectType.UnKnown).ToString())
                {
                    exLand.GBLX = ePlantProtectionType.Unknown;
                }
                // 高程
                if (land.LandExpand != null)
                {
                    exLand.GCJZ = land.LandExpand.Elevation;
                }
                exLand.SZ = land.NeighborEast + "#" + land.NeighborSouth + "#" + land.NeighborWest + "#" + land.NeighborNorth;
                exLand.ZJRXM = land.LandExpand != null ? land.LandExpand.ReferPerson : "";//指界人
                exLand.DCY = land.LandExpand != null ? land.LandExpand.SurveyPerson : "";
                exLand.DCRQ = land.LandExpand != null ? land.LandExpand.SurveyDate : null;
                exLand.DCYJ = land.LandExpand != null ? land.LandExpand.SurveyChronicle : "";
                exLand.DCSHR = land.LandExpand != null ? land.LandExpand.CheckPerson : "";
                exLand.DCSHRQ = land.LandExpand != null ? land.LandExpand.CheckDate : null;
                exLand.DCSHYJ = land.LandExpand != null ? land.LandExpand.CheckOpinion : "";
                exLand.TFBH = land.LandExpand != null ? land.LandExpand.ImageNumber : "";
                exLand.QDFS = land.ConstructMode;
                exLand.DKBZXX = land.Comment;
                InitalizeLandExpandInformation(exLand, land);
                return exLand;
            }
            catch (Exception errorinfo)
            {
                var err = errorinfo.Message;
                return exLand;
            }
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeLandExpandInformation(EX_CBJYQ_CBD exLand, ContractLand land)
        {
            string landString = land.LandCategory;//地块类别
            landString += "|";
            landString += land.PlotNumber;//畦数
            landString += "|";
            landString += land.IsTransfer ? "1" : "0";//是否流转
            landString += "|";
            landString += land.TransferType;//流转方式
            landString += "|";
            landString += land.PertainToArea.ToString();//流转面积
            landString += "|";
            landString += land.TransferTime;//流转期限
            exLand.LandCatalog = landString;
        }

        /// <summary>
        /// 初始化界址点信息
        /// </summary>
        /// <returns></returns>
        private List<DJ_CBJYQ_JZD_GEO> InitalizeDotInformation(EX_CBJYQ_DJB regBook, ContractLand land, SpatialReference reference)
        {
            List<BuildLandBoundaryAddressDot> dotCollection = ArgicultureEntity.DotCollection.FindAll(dt => dt.LandID == land.ID);
            List<DJ_CBJYQ_JZD_GEO> jzds = new List<DJ_CBJYQ_JZD_GEO>();
            foreach (BuildLandBoundaryAddressDot dot in dotCollection)
            {
                GeoAPI.Geometries.IPoint point = null;
                if (dot.Shape != null)
                {
                    point = dot.Shape.Instance as GeoAPI.Geometries.IPoint;
                }
                DJ_CBJYQ_JZD_GEO jzd = new DJ_CBJYQ_JZD_GEO();
                jzd.ID = dot.ID;
                jzd.DJBID = regBook.ID;
                jzd.DKID = dot.LandID;
                jzd.JBLX = dot.LandMarkType.ToString();
                jzd.JZDH = dot.DotNumber;
                jzd.JZDLX = dot.DotType.ToString();
                jzd.TBJZDH = dot.UniteDotNumber;
                if (point != null)
                {
                    jzd.X = point.X;
                    jzd.Y = point.Y;
                }
                if (jzd.X != null && jzd.X.HasValue && jzd.Y != null && jzd.Y.HasValue)
                {
                    try
                    {
                        byte[] wkb = null;
                        if (dot.Shape != null)
                        {
                            wkb = dot.Shape.ToBytes();
                        }
                        if (wkb != null && wkb.Length > 0)
                        {
                            jzd.Shape = new GeometryWellKnownValue { CoordinateSystemId = (reference != null ? reference.WKID : 2380), WellKnownBinary = wkb };
                        }
                    }
                    catch (SystemException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
                jzds.Add(jzd);
            }
            dotCollection = null;
            return jzds;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private List<DJ_CBJYQ_JZX_GEO> InitalizeLineInformation(EX_CBJYQ_DJB regBook, ContractLand land, SpatialReference reference)
        {
            List<BuildLandBoundaryAddressCoil> lineCollection = ArgicultureEntity.LineCollection.FindAll(dt => dt.LandID == land.ID);
            List<BuildLandBoundaryAddressDot> dotCollection = ArgicultureEntity.DotCollection.FindAll(dt => dt.LandID == land.ID);
            List<DJ_CBJYQ_JZX_GEO> jzxs = new List<DJ_CBJYQ_JZX_GEO>();
            int index = 1;
            foreach (BuildLandBoundaryAddressCoil line in lineCollection)
            {
                DJ_CBJYQ_JZX_GEO jzx = new DJ_CBJYQ_JZX_GEO();
                jzx.ID = line.ID;
                jzx.DJBID = regBook.ID;
                jzx.DKID = line.LandID;
                jzx.SDID = line.StartPointID;
                jzx.ZDID = line.EndPointID;
                jzx.JXXZ = line.LineType;
                jzx.JZXLB = line.CoilType;
                jzx.JZXWZ = line.Position;
                jzx.JZXSM = line.Description;
                jzx.PLDWQLR = line.NeighborPerson;
                jzx.PLDWZJR = line.NeighborFefer;
                jzx.JXCD = line.CoilLength;
                jzx.SXH = index.ToString();
                var startPoint = dotCollection.Find(dt => dt.ID == line.StartPointID);
                var endPoint = dotCollection.Find(dt => dt.ID == line.EndPointID);
                if (startPoint != null && endPoint != null && startPoint.Shape != null && endPoint.Shape != null)
                {
                    GeoAPI.Geometries.IPoint pointStart = startPoint.Shape.Instance as GeoAPI.Geometries.IPoint;
                    GeoAPI.Geometries.IPoint pointEnd = endPoint.Shape.Instance as GeoAPI.Geometries.IPoint;
                    var lineString = String.Format("LINESTRING({0} {1},{2} {3})#{4}", pointStart.X, pointStart.Y,
                       pointEnd.X, pointEnd.Y, reference != null ? reference.WKID : 2380);
                    var le = YuLinTu.Spatial.Geometry.FromString(lineString);
                    jzx.Shape = le;
                }
                jzxs.Add(jzx);
                index++;
            }
            lineCollection = null;
            dotCollection = null;
            return jzxs;
        }

        /// <summary>
        ///  转换生成利用现状信息
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<DJ_CBJYQ_LYXZ> TransformResUtilizations(List<ContractLand> lands)
        {
            List<DJ_CBJYQ_LYXZ> lyxzCollection = new List<DJ_CBJYQ_LYXZ>();
            foreach (ContractLand land in lands)
            {
                DJ_CBJYQ_LYXZ lyxz = new DJ_CBJYQ_LYXZ();
                lyxz.DKID = land.ID;
                lyxz.ZZLX = land.PlatType;
                lyxz.DKMJ = land.ActualArea;
                if (ContractLand.GetLandNumber(land.CadastralNumber).Length == 19)
                {
                    lyxz.DKBM = ContractLand.GetLandNumber(land.CadastralNumber);
                }
                else
                {
                    lyxz.DKBM = string.IsNullOrEmpty(land.LandExpand.AgricultureNumber) ? InitalizeAgricultureNumber(land.LocationCode, ContractLand.GetLandNumber(land.CadastralNumber)) : land.LandExpand.AgricultureNumber;
                }
                if (land.LandExpand != null)
                {
                    lyxz.SYQK = land.LandExpand.IncomeSituation;
                    lyxz.LYQK = land.LandExpand.UseSituation;
                    float cl = 0, cz = 0;
                    Single.TryParse(land.LandExpand.Yield, out cl);
                    Single.TryParse(land.LandExpand.OutputValue, out cz);
                    lyxz.CL = cl;
                    lyxz.CZ = cz;
                    lyxz.DKMJ = land.ActualArea;
                    if (land.IsFarmerLand != null)
                    {
                        lyxz.SFJBNT = land.IsFarmerLand.GetValueOrDefault(true);
                    }
                    lyxz.SFLHD = land.ConstructMode == ((int)eLandCategoryType.AbandonedLand).ToString() ? true : false;
                    lyxz.SZDY = land.LocationCode;
                }
                lyxz.TJSJ = land.ModifiedTime ?? DateTime.Now;
                lyxz.DCRQ = lyxz.TJSJ;
                if (lyxz.CL == 0 && lyxz.CZ == 0)
                {
                    continue;
                }
                lyxzCollection.Add(lyxz);
            }
            return lyxzCollection;
        }

        /// <summary>
        /// 初始化农业部编码
        /// </summary>
        /// <returns></returns>
        private string InitalizeAgricultureNumber(string code, string landNumber)
        {
            string zoneCode = code;

            switch (zoneCode.Length)
            {
                case Zone.ZONE_VILLAGE_LENGTH:
                    zoneCode = code + "00";
                    break;

                case Zone.ZONE_GROUP_LENGTH:
                    zoneCode = code.Substring(0, 12) + code.Substring(14);
                    //if (!IsStandCode)
                    //{
                    //    zoneCode = code.Substring(0, 12) +"00"+ code.Substring(14);
                    //}
                    break;

                default:
                    break;
            }
            if (IsFamilyCoding)
            {
                return zoneCode + string.Format("{0:D5}", landIndex);
            }
            if (string.IsNullOrEmpty(landNumber))
            {
                return "";
            }
            string parcelNumber = landNumber;
            int index = landNumber.IndexOf("J");
            if (index > 0)
            {
                parcelNumber = landNumber.Substring(index + 1);
            }
            index = landNumber.IndexOf("Q");
            if (index > 0)
            {
                parcelNumber = landNumber.Substring(index + 1);
            }
            if (string.IsNullOrEmpty(parcelNumber))
            {
                return landNumber;
            }
            if (parcelNumber.Length > 14)
            {
                parcelNumber = parcelNumber.Substring(14);
            }
            parcelNumber = InitalizeLandNumber(parcelNumber);
            return zoneCode + parcelNumber;
        }

        /// <summary>
        /// 初始化地块编码
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        private string InitalizeLandNumber(string parcelNumber)
        {
            int length = parcelNumber.Length;
            string landNumber = string.Empty;
            switch (length)
            {
                case 1:
                    landNumber = "0000";
                    break;

                case 2:
                    landNumber = "000";
                    break;

                case 3:
                    landNumber = "00";
                    break;

                case 4:
                    landNumber = "0";
                    break;
            }
            landNumber += parcelNumber;
            return landNumber;
        }

        /// <summary>
        /// 发包方映射
        /// </summary>
        private void AgricultureSenderMapping(EX_CBJYQ_DJB regBook, ContractConcord concord)
        {
            if (TissueCollection == null)
            {
                TissueCollection = new List<CollectivityTissue>();
            }
            var tissueStation = DataInstance.CreateSenderWorkStation();
            CollectivityTissue tissue = null;//集体经济组织
            if (concord == null || concord.SenderId == null || string.IsNullOrEmpty(concord.SenderName))
            {
                tissue = TissueCollection.Find(ts => ts.ID == ArgicultureEntity.CurrentZone.ID || ts.ZoneCode == ArgicultureEntity.CurrentZone.UpLevelCode);
                if (tissue == null && DataInstance != null)
                {
                    tissue = tissueStation.Get(ArgicultureEntity.CurrentZone.ID);
                    if (tissue == null)
                    {
                        tissue = tissueStation.Get(ArgicultureEntity.CurrentZone.FullCode, ArgicultureEntity.CurrentZone.FullName);
                    }
                    if (tissue == null)
                    {
                        var tissues = tissueStation.GetTissues(ArgicultureEntity.CurrentZone.FullCode, eLevelOption.Self);
                        if (tissues != null && tissues.Count > 0)
                            tissue = tissues[0];
                    }
                    //if (tissue == null)
                    //{
                    //    Zone zone = DataInstance.CreateZoneWorkStation().Get(ArgicultureEntity.CurrentZone.UpLevelCode);
                    //    tissue = zone != null ? tissueStation.Get(zone.ID) : null;
                    //    zone = null;
                    //}
                    if (tissue != null && TissueCollection.Find(ts => ts.ID == tissue.ID) == null)
                    {
                        TissueCollection.Add(tissue);
                    }
                }
            }
            else
            {
                tissue = TissueCollection.Find(ts => ts.ID == concord.SenderId);
                if (tissue == null && DataInstance != null)
                {
                    tissue = tissueStation.Get(concord.SenderId);
                    if (tissue != null)
                    {
                        TissueCollection.Add(tissue);
                    }
                }
            }
            if (tissue == null)
            {
                return;
            }
            string zoneCode = tissue.ZoneCode;
            if (!string.IsNullOrEmpty(zoneCode))
            {
                zoneCode = zoneCode.PadRight(14, '0');
                zoneCode = zoneCode.Length == 16 ? (zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2)) : zoneCode;
            }
            InitalizeSenderInformation(regBook, zoneCode, tissue);
            tissue = null;
        }

        /// <summary>
        /// 初始化发包方信息
        /// </summary>
        /// <param name="regBook"></param>
        private void InitalizeSenderInformation(EX_CBJYQ_DJB regBook, string senderCode, CollectivityTissue tissue)
        {
            regBook.FBF = new EX_CBJYQ_FBF
            {
                ID = tissue.ID,
                BM = tissue.Code.Length == 14 ? tissue.Code : senderCode,
                YBM = tissue.Code,
                DCJS = tissue.SurveyChronicle,
                DCRQ = tissue.SurveyDate,
                DCY = tissue.SurveyPerson,
                DZ = tissue.LawyerAddress,
                FZRXM = tissue.LawyerName,
                FZRZJHM = tissue.LawyerCartNumber,
                FZRZJLX = tissue.LawyerCredentType == 0 ? ((int)eCredentialsType.IdentifyCard).ToString() : ((int)tissue.LawyerCredentType).ToString(),
                LXDH = tissue.LawyerTelephone,
                SHR = tissue.CheckPerson,
                SHRQ = tissue.CheckDate,
                SHYJ = tissue.CheckOpinion,
                MC = tissue.Name,
                SZDY = tissue.ZoneCode,
                YZBM = tissue.LawyerPosterNumber,
                BZ = tissue.Comment,
            };
            regBook.FBFQC = regBook.FBF.MC;
            regBook.FBFBM = regBook.FBF.BM;
            regBook.YFBFBM = regBook.FBF.YBM;
            if (regBook.FBFBM.IsNullOrWhiteSpace() || regBook.FBFBM.Length > JTJJZZ.CODE_LENGTH)
            {
                regBook.FBFBM = JTJJZZ.GenerateCodeFromZoneCode(tissue.ZoneCode);
            }
        }

        /// <summary>
        /// 合同映射
        /// </summary>
        private void AgricultureConcordMapping(EX_CBJYQ_DJB regBook, VirtualPerson vp, ContractConcord concord)
        {
            regBook.ID = concord.ID;
            regBook.CBHTQDRQ = concord.ContractDate;
            regBook.CBKSRQ = concord.ArableLandStartTime;
            regBook.CBJSRQ = concord.ArableLandEndTime;
            regBook.CBFS = concord.ArableLandType;
            if (concord.Flag) regBook.CBJSRQ = null; // 承包期限为长久
            regBook.FBFQC = concord.SenderName;
            regBook.HTBH = InitalizeAgricultureConcordNumber(vp, concord.ConcordNumber);
            regBook.YHTBH = concord.ConcordNumber;
            regBook.HTID = concord.ID;
            regBook.QZBH = regBook.HTBH;//权证编号与合同编号一致
            regBook.YQZBH = concord.ConcordNumber;
            regBook.TDCBYT = (string.IsNullOrEmpty(concord.LandPurpose)) ? ((int)YuLinTu.Library.Entity.eLandPurposeType.Planting).ToString() : concord.LandPurpose;
            regBook.GSJS = concord.PublicityChronicle;
            regBook.GSJSR = concord.PublicityChroniclePerson;
            regBook.GSJSRQ = concord.PublicityDate;
            regBook.CBFGSJGQRR = concord.PublicityContractor;
            regBook.CBFGSJGQRRQ = concord.PublicityResultDate;
            regBook.CBFGSJGQRYJ = concord.PublicityResult;
            regBook.GSSHR = concord.PublicityCheckPerson;
            regBook.GSSHRQ = concord.PublicityCheckDate;
            regBook.GSSHYJ = concord.PublicityCheckOpinion;
            concord.ConcordNumber = regBook.HTBH;
        }

        /// <summary>
        /// 合同映射
        /// </summary>
        private void AgricultureUnConcordMapping(EX_CBJYQ_DJB regBook, ContractConcord concord)
        {
            if (regBook == null || concord == null)
            {
                return;
            }
            regBook.CBHTQDRQ = concord.ContractDate;
            regBook.CBKSRQ = concord.ArableLandStartTime;
            regBook.CBJSRQ = concord.ArableLandEndTime;
            regBook.CBFS = concord.ArableLandType;
            if (concord.Flag) regBook.CBJSRQ = null; // 承包期限为长久
            regBook.FBFQC = concord.SenderName;
            regBook.TDCBYT = string.IsNullOrEmpty(concord.LandPurpose) ? ((int)YuLinTu.Library.Entity.eLandPurposeType.Planting).ToString() : concord.LandPurpose;
            regBook.GSJS = concord.PublicityChronicle;
            regBook.GSJSR = concord.PublicityChroniclePerson;
            regBook.GSJSRQ = concord.PublicityDate;
            regBook.CBFGSJGQRR = concord.PublicityContractor;
            regBook.CBFGSJGQRRQ = concord.PublicityResultDate;
            regBook.CBFGSJGQRYJ = concord.PublicityResult;
            regBook.GSSHR = concord.PublicityCheckPerson;
            regBook.GSSHRQ = concord.PublicityCheckDate;
            regBook.GSSHYJ = concord.PublicityCheckOpinion;
        }

        /// <summary>
        /// 申请书映射
        /// </summary>
        private void AgricultureRequirBookMapping(EX_CBJYQ_DJB regBook, ContractConcord concord)
        {
            if (concord == null || concord.RequireBookId == null || !concord.RequireBookId.HasValue)
            {
                return;
            }
            Guid requireId = concord.RequireBookId.Value;
            ContractRequireTable requireTable = DataInstance.CreateRequireTableWorkStation().Get(concord.RequireBookId.Value);
            if (requireTable == null)
            {
                return;
            }
            DJ_CBJYQ_SQS sqs = new DJ_CBJYQ_SQS();
            sqs.DJBID = regBook.ID;
            sqs.LX = YuLinTu.PropertyRight.Registration.eApplicationType.InitializationRegister;
            sqs.SQFS = YuLinTu.PropertyRight.eApplyMode.SingleFamily;
            sqs.BH = requireTable.Number;
            sqs.SQR = concord.SenderName;
            sqs.SQRQ = DateTime.Now;
            regBook.DJSQS = sqs;
        }

        /// <summary>
        /// 权证映射
        /// </summary>
        private void AgricultureBookMapping(EX_CBJYQ_DJB regBook, ContractConcord concord)
        {
            ContractRegeditBook book = ArgicultureEntity.Books.Find(bk => bk.ID == concord.ID);
            if (book == null)
            {
                book = DataInstance.CreateRegeditBookStation().Get(concord.ID);
            }
            if (book == null)
            {
                return;
            }
            if (book != null && string.IsNullOrEmpty(book.RegeditNumber))
            {
                return;
            }
            regBook.QZ = new DJ_CBJYQ_QZXX
            {
                CJSJ = DateTime.Now,
                DJBID = book.ID,
                DZRQ = book.WriteDate,
                FZJGJC = book.SendOrganization,
                FZRQ = book.SendDate,
                QZDZH = book.SerialNumber,
                QZNH = book.Year,
            };

            regBook.QZBH = concord.ConcordNumber;    // 如果有权证则权证编号应为对应权证的编号
            regBook.BZ = book.ContractRegeditBookExcursus;
            regBook.YQZBH = book.RegeditNumber == concord.ConcordNumber ? "" : book.RegeditNumber;
            book = null;
        }

        /// <summary>
        /// 初始化农业部编码
        /// </summary>
        /// <returns></returns>
        private string InitalizeAgricultureConcordNumber(VirtualPerson contractor, string concordNumber)
        {
            if (contractor == null)
            {
                return concordNumber;
            }
            if (!string.IsNullOrEmpty(concordNumber) && concordNumber.Length == 19)
            {
                return concordNumber;
            }
            string zoneCode = contractor.ZoneCode;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_VILLAGE_LENGTH:
                    zoneCode = contractor.ZoneCode + "00";
                    break;

                case Zone.ZONE_GROUP_LENGTH:
                    zoneCode = contractor.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + contractor.ZoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
                    break;

                default:
                    break;
            }
            int number = 0;
            Int32.TryParse(contractor.FamilyNumber, out number);
            string familyMode = "J";
            int index = concordNumber.LastIndexOf("J");
            if (index > 0)
            {
                familyMode = "J";
            }
            index = concordNumber.LastIndexOf("Q");
            if (index > 0)
            {
                familyMode = "Q";
            }
            return zoneCode + string.Format("{0:D4}", number) + familyMode;
        }

        /// <summary>
        /// 初始化权证编号
        /// </summary>
        /// <param name="vp"></param>
        /// <returns></returns>
        public string InitalizeWarrantNumber(VirtualPerson vp)
        {
            if (vp == null)
            {
                return "";
            }
            string zoneCode = vp.ZoneCode;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_VILLAGE_LENGTH:
                    zoneCode = vp.ZoneCode + "00";
                    break;

                case Zone.ZONE_GROUP_LENGTH:
                    zoneCode = vp.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + vp.ZoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
                    break;

                default:
                    break;
            }
            int number = 0;
            Int32.TryParse(vp.FamilyNumber, out number);
            return zoneCode + string.Format("{0:D4}", number) + "J";
        }

        /// <summary>
        /// 初始化申请书
        /// </summary>
        /// <param name="djbs"></param>
        /// <returns></returns>
        private DJ_CBJYQ_SQS InitalizetionRequireBook(EX_CBJYQ_DJB djb, string description)
        {
            return new DJ_CBJYQ_SQS
            {
                DJBID = djb.ID,
                LX = PropertyRight.Registration.eApplicationType.ChangeRegister,
                QZSFYHS = true,
                SQSX = description,
                SQR = djb.CBFMC,
                SQRQ = DateTime.Now,
                SQFS = PropertyRight.eApplyMode.SingleFamily,
                BH = (IsOnLine && LandService != null) ? LandService.GetApplicationBookNumber(djb.SZDY, PropertyRight.Registration.eApplicationType.ChangeRegister) : ""
            };
        }

        /// <summary>
        /// 初始化申请书
        /// </summary>
        /// <param name="djbs"></param>
        /// <returns></returns>
        private DJ_CBJYQ_SQS InializeRequireBookInformation(EX_CBJYQ_DJB djb, PropertyRight.Registration.eApplicationType appType, string description)
        {
            return new DJ_CBJYQ_SQS
            {
                DJBID = djb.ID,
                LX = appType,
                QZSFYHS = appType == PropertyRight.Registration.eApplicationType.InitializationRegister ? false : true,
                SQSX = description,
                SQR = djb.CBFMC,
                SQRQ = DateTime.Now,
                SQFS = PropertyRight.eApplyMode.SingleFamily,
                BH = (IsOnLine && LandService != null) ? LandService.GetApplicationBookNumber(djb.SZDY, PropertyRight.Registration.eApplicationType.ChangeRegister) : ""
            };
        }

        /// <summary>
        /// 报告信息
        /// </summary>
        private void ReportInformation(string record)
        {
            if (string.IsNullOrEmpty(recordInformation))
            {
                recordInformation = record;
            }
            if (recordInformation.Contains(record))
            {
                return;
            }
            recordInformation += record;
            InformationReportged(record);
        }

        /// <summary>
        /// 报告错误信息
        /// </summary>
        private void ReportErrorInformation(string record)
        {
            if (string.IsNullOrEmpty(errorInformation))
            {
                errorInformation = record;
                ErrorReportged(record);
                return;
            }
            if (errorInformation.Contains(record))
            {
                return;
            }
            errorInformation += record;
            ErrorReportged(record);
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            recordInformation = null;
            errorInformation = null;
            GC.Collect();
        }

        #endregion 数据映射
    }
}