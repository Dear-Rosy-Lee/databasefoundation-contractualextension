/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using YuLinTu.PropertyRight.ContractLand;
using YuLinTu.Library.Business;
using YuLinTu.PropertyRight.Services.Client;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包经营权信息映射
    /// </summary>
    public class AgricultureBookMapping
    {
        #region Fields

        #endregion

        #region Propertys

        /// <summary>
        /// 数据实例
        /// </summary>
        public IDbContext DataInstance { get; set; }

        /// <summary>
        /// 是否导入数据
        /// </summary>
        public bool ImportData { get; set; }

        /// <summary>
        /// 数据实体
        /// </summary>
        public AgriLandEntity LandEntity { get; set; }

        /// <summary>
        /// 是否业务
        /// </summary>
        public bool IsBusiness { get; set; }

        /// <summary>
        /// SRID
        /// </summary>
        public int Srid { get; set; }

        private readonly SystemSetDefine SystemSet;


        #endregion

        #region Ctor

        public AgricultureBookMapping()
        {
            SystemSet = SystemSetDefine.GetIntence();
        }

        #endregion

        #region 数据映射

        /// <summary>
        ///  生成登记簿
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public bool InitalizeLocalData(EX_CBJYQ_DJB djb)
        {
            if (djb == null)
            {
                return false;
            }
            VirtualPerson vp = AgriContractorMapping(djb);//承包方信息
            bool canContinue = AgriContractorProgress(vp);
            if (!canContinue)
            {
                return false;
            }
            CollectivityTissue tissue = AgricultureSenderMapping(djb);//集体经济组织
            tissue = InitalzieTissueInformation(tissue, djb.FBF);
            AgricultureSenderProgress(tissue);
            ContractConcord concord = AgricultureConcordMapping(djb);//合同信息
            Dictionary<string, double> landareainfo = new Dictionary<string, double>();
            List<ArcGisLandComplex> landCollection = AgricultureLandMapping(djb, vp, concord, Srid, landareainfo);//承包地块信息
            AgricultureLandProgress(landCollection);
            if (concord != null)
            {
                concord.TotalTableArea = landareainfo.Where(ld => ld.Key == "台账面积").First().Value;
                concord.CountActualArea = landareainfo.Where(ld => ld.Key == "实测面积").First().Value;
                concord.CountAwareArea = landareainfo.Where(ld => ld.Key == "确权面积").First().Value;

                concord.SenderId = tissue != null ? tissue.ID : Guid.Empty;
                concord.SenderName = tissue != null ? tissue.Name : "";
                ContractRequireTable requireTable = AgricultureRequirBookMapping(djb);//申请书
                if (requireTable != null)
                {
                    concord.RequireBookId = requireTable.ID;
                }
                AgricultureRequireTableProgress(requireTable);
                AgricultureConcordProgress(concord);
                ContractRegeditBook book = AgricultureWarrantMapping(djb);//权证
                AgricultureBookProgress(book);
            }

            return true;
        }

        /// <summary>
        /// 承包方数据处理
        /// </summary>
        /// <param name="vp"></param>
        private bool AgriContractorProgress(VirtualPerson vp)
        {
            if (vp == null)
            {
                return false;
            }
            var personStation = DataInstance.CreateVirtualPersonStation<LandVirtualPerson>();
            if (ImportData)
            {
                if (LandEntity.Contractors.Find(fam => fam.ID == vp.ID) != null)
                {
                    return false;
                }
                LandEntity.Contractors.Add(vp);
                personStation.Add(vp);
                return true;
            }
            var family = LandEntity.Contractors.Find(fam => fam.ID == vp.ID || (fam.Name == vp.Name && fam.ZoneCode == vp.ZoneCode));
            if (family != null)
            {
                vp.ID = family.ID;
            }
            if (family != null && family.Status == eVirtualPersonStatus.Lock)
            {
                return false;
            }
            var dbPerson = personStation.Get(t => (t.ID == vp.ID) || (t.Name == vp.Name && t.ZoneCode == vp.ZoneCode)).FirstOrDefault();
            if (dbPerson == null)
            {
                personStation.Add(vp);
            }
            else
            {
                vp.ID = dbPerson.ID;
                personStation.Update(vp);
            }
            return true;
        }

        /// <summary>
        /// 承包地块处理
        /// </summary>
        private void AgricultureLandProgress(List<ArcGisLandComplex> landCollection)
        {
            try
            {
                if (landCollection == null || landCollection.Count == 0)
                {
                    return;
                }
                var landStation = DataInstance.CreateContractLandWorkstation();
                foreach (ArcGisLandComplex land in landCollection)
                {
                    if (ImportData)
                    {
                        if (LandEntity.Lands.Find(ld => (ld.ID == land.ContractorLand.ID || ld.CadastralNumber == land.ContractorLand.CadastralNumber)) != null)
                        {
                            continue;
                        }
                        LandEntity.Lands.Add(land.ContractorLand);
                        int addCount = landStation.Add(land.ContractorLand);
                        AgricultureDotProgress(land);
                        continue;
                    }
                    var landComplex = LandEntity.Lands.Find(ld => (ld.ID == land.ContractorLand.ID || ContractLand.GetLandNumber(ld.CadastralNumber) == ContractLand.GetLandNumber(land.ContractorLand.CadastralNumber)));
                    if (landComplex != null)
                    {
                        land.ContractorLand.ID = landComplex.ID;
                    }
                    if (landStation.Update(land.ContractorLand) <= 0)
                    {
                        landStation.Add(land.ContractorLand);
                    }
                    AgricultureDotProgress(land);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 界址点\线处理
        /// </summary>
        private void AgricultureDotProgress(ArcGisLandComplex land)
        {
            var dotStation = DataInstance.CreateBoundaryAddressDotWorkStation();
            var coiltation = DataInstance.CreateBoundaryAddressCoilWorkStation();
            dotStation.Delete(t => t.LandID == land.ContractorLand.ID);
            coiltation.Delete(t => t.LandID == land.ContractorLand.ID);
            foreach (BuildLandBoundaryAddressDot dot in land.DotCollection)
            {
                if (dot.ZoneCode.Length == 16)
                {
                    dot.ZoneCode = dot.ZoneCode.Substring(0, 12) + dot.ZoneCode.Substring(14, 2);
                }
                if (LandEntity.DotCollection.Find(dt => dt.ID == dot.ID) != null)
                {
                    continue;
                }
                dotStation.Add(dot);
                LandEntity.DotCollection.Add(dot);
            }
            foreach (BuildLandBoundaryAddressCoil line in land.LineCollection)
            {
                if (line.ZoneCode.Length == 16)
                {
                    line.ZoneCode = line.ZoneCode.Substring(0, 12) + line.ZoneCode.Substring(14, 2);
                }
                if (LandEntity.LineCollection.Find(le => le.ID == line.ID) != null)
                {
                    continue;
                }
                coiltation.Add(line);
                LandEntity.LineCollection.Add(line);
            }
        }

        /// <summary>
        /// 申请书处理
        /// </summary>
        private void AgricultureRequireTableProgress(ContractRequireTable requireTable)
        {
            if (requireTable == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(requireTable.Number))
            {
                return;
            }
            var requireStation = DataInstance.CreateRequireTableWorkStation();
            if (ImportData)
            {
                if (LandEntity.Tables.Find(dt => dt.ID == requireTable.ID) != null)
                {
                    return;
                }
                LandEntity.Tables.Add(requireTable);
                requireStation.Add(requireTable);
                return;
            }
            var requeTable = LandEntity.Tables.Find(tb => tb.ID == requireTable.ID || tb.Number == requireTable.Number);
            if (requeTable != null)
            {
                requireTable.ID = requeTable.ID;
            }
            if (requireStation.Update(requireTable) <= 0)
            {
                requireStation.Add(requireTable);
            }
        }

        /// <summary>
        /// 发包方处理
        /// </summary>
        private void AgricultureSenderProgress(CollectivityTissue tissue)
        {
            if (tissue == null)
            {
                return;
            }
            if (LandEntity.Tissues.Find(te => te.ID == tissue.ID) != null)
            {
                return;
            }
            if (ImportData)
            {
                LandEntity.Tissues.Add(tissue);
            }
            var tissueStation = DataInstance.CreateSenderWorkStation();
            var sender = LandEntity.Tissues.Find(te => te.Code == tissue.Code);
            if (sender != null)
            {
                tissue.ID = sender.ID;
            }
            if (tissueStation.Update(tissue) <= 0)
            {
                tissueStation.Add(tissue);
            }
        }

        /// <summary>
        /// 合同处理
        /// </summary>
        private void AgricultureConcordProgress(ContractConcord concord)
        {
            if (concord == null)
            {
                return;
            }
            var concordStation = DataInstance.CreateConcordStation();
            if (ImportData)
            {
                if (LandEntity.Concords.Find(cd => cd.ID == concord.ID) != null)
                {
                    return;
                }
                LandEntity.Concords.Add(concord);
                concordStation.Add(concord);
                return;
            }
            var concrod = LandEntity.Concords.Find(cd => cd.ID == concord.ID || cd.ConcordNumber == concord.ConcordNumber);
            if (concrod != null)
            {
                concord.ID = concrod.ID;
            }
            if (concordStation.Update(concord) <= 0)
            {
                concordStation.Add(concord);
            }
        }

        /// <summary>
        /// 申请书处理
        /// </summary>
        private void AgricultureBookProgress(ContractRegeditBook book)
        {
            if (book == null)
            {
                return;
            }
            var bookStation = DataInstance.CreateRegeditBookStation();
            if (ImportData)
            {
                if (LandEntity.Books.Find(bk => bk.ID == book.ID) != null)
                {
                    return;
                }
                LandEntity.Books.Add(book);
                bookStation.Add(book);
                return;
            }
            var regBook = LandEntity.Books.Find(bk => bk.ID == book.ID || bk.RegeditNumber == book.RegeditNumber);
            if (regBook != null)
            {
                book.ID = regBook.ID;
            }
            if (bookStation.Update(book) <= 0)
            {
                bookStation.Add(book);
            }
        }

        /// <summary>
        /// 承包方信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private VirtualPerson AgriContractorMapping(EX_CBJYQ_DJB regBook)
        {
            VirtualPerson vp = LandEntity.Contractors.Find(fam => fam.ID == regBook.CBFID);
            if (vp == null)
            {
                vp = new VirtualPerson();
            }
            vp.ID = regBook.CBFID;
            vp.Telephone = regBook.CBFLXDH;
            vp.Name = regBook.CBFMC;
            int familyNumber = 0;
            Int32.TryParse(regBook.CBFBH.Substring(regBook.CBFBH.Length - 4), out familyNumber);
            vp.FamilyNumber = familyNumber.ToString();
            vp.CardType = (eCredentialsType)Enum.Parse(typeof(eCredentialsType), regBook.CBFZJLX);
            vp.Number = regBook.CBFZJHM;
            vp.PostalNumber = regBook.CBFYB;
            vp.Address = regBook.CBFZZ;
            vp.ZoneCode = regBook.SZDY;
            if (regBook.SZDY.Length == 16)
            {
                vp.ZoneCode = regBook.SZDY.Substring(0, 12) + regBook.SZDY.Substring(14, 2);
            }
            vp.Telephone = regBook.CBFLXDH;
            vp.Comment = regBook.CBFBZ;
            AgriSharePersonMapping(vp, regBook);
            AgriContractorExpandMapping(vp, regBook);
            return vp;
        }

        /// <summary>
        /// 承包方扩展信息映射
        /// </summary>
        private void AgriContractorExpandMapping(VirtualPerson vp, EX_CBJYQ_DJB regBook)
        {
            VirtualPersonExpand familyExpand = vp.FamilyExpand;
            //VirtualPersonExpand.CreateExpandByXml(regBook.FamilyComment);
            familyExpand.SurveyPerson = regBook.CBFDCY;
            familyExpand.SurveyChronicle = regBook.CBFDCJS;
            familyExpand.SurveyDate = regBook.CBFDCRQ;
            familyExpand.CheckDate = regBook.CBFDCSHRQ;
            familyExpand.CheckOpinion = regBook.CBFDCSHYJ;
            familyExpand.CheckPerson = regBook.CBFDCSHR;
            familyExpand.PublicityDate = regBook.CBFGSJGQRRQ;
            familyExpand.PublicityChronicle = regBook.CBFGSJGQRYJ;
            familyExpand.PublicityChroniclePerson = regBook.CBFGSJGQRR;
            familyExpand.BusinessStatus = IsBusiness ? eBusinessStatus.End : eBusinessStatus.UnKnown;
            familyExpand.ContractorType = (YuLinTu.Library.Entity.eContractorType)Enum.Parse(typeof(YuLinTu.Library.Entity.eContractorType), regBook.CBFLX);
            vp.FamilyExpand = familyExpand;
        }

        /// <summary>
        /// 共有人信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgriSharePersonMapping(VirtualPerson vp, EX_CBJYQ_DJB regBook)
        {
            List<Person> list = vp.SharePersonList;
            if (regBook.DJGYR == null && list.Count == 0)
            {
                Person sharePerson = new Person();
                sharePerson.ID = vp.ID;
                sharePerson.FamilyID = vp.ID;
                sharePerson.Birthday = ToolICN.GetBirthday(vp.Number);
                sharePerson.Gender = ToolICN.GetGender(vp.Number) == 0 ? eGender.Female : eGender.Male;
                sharePerson.Name = vp.Name;
                sharePerson.Relationship = "户主";
                sharePerson.ICN = vp.Number;
                sharePerson.CardType = vp.CardType;
                sharePerson.IsSharedLand = "是";
                sharePerson.ZoneCode = vp.ZoneCode;
                list.Add(sharePerson);
                vp.SharePersonList = list;
                return;
            }
            list = new List<Person>();
            foreach (DJ_CBJYQ_JTCBGYR person in regBook.DJGYR)
            {
                Person sharePerson = new Person();
                sharePerson.FamilyID = vp.ID;
                sharePerson.Comment = LandCategoryMapping.SharePersonCommentCodeMapping(person.BZ);
                if (!string.IsNullOrEmpty(person.Comment))
                {
                    string[] val = person.Comment.Split(new char[] { '|' });
                    if (val != null && val.Length > 0)
                    {
                        object obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eLandCategoryType), val[0]);
                        sharePerson.Nation = obj == null ? eNation.UnKnown : (eNation)(obj);
                    }
                    sharePerson.Comment = (val != null && val.Length > 1) ? val[1] : "";
                }
                sharePerson.Birthday = person.CSRQ;
                sharePerson.ID = person.ID;
                sharePerson.Gender = LandCategoryMapping.GenderCodeMapping(person.XB);
                sharePerson.Name = person.XM;
                if (string.IsNullOrEmpty(sharePerson.Comment))
                {
                    sharePerson.Comment = person.BZSM;
                }
                string ship = RelationShipMapping.CodeMapping(person.YHZGX);
                sharePerson.Relationship = ship == "其他" ? "" : ship;
                if (!string.IsNullOrEmpty(person.Relationship))
                {
                    sharePerson.Relationship = person.Relationship;
                }
                sharePerson.ICN = person.ZJHM;
                sharePerson.ZoneCode = vp.ZoneCode;
                sharePerson.CardType = (eCredentialsType)Enum.Parse(typeof(eCredentialsType), person.ZJLX);
                sharePerson.IsSharedLand = person.SFGYR ? "是" : "否";
                if (!list.Any(t => t.Name == sharePerson.Name && t.ICN == sharePerson.ICN))
                    list.Add(sharePerson);
            }
            vp.SharePersonList = list;
        }

        /// <summary>
        /// 地块信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private List<ArcGisLandComplex> AgricultureLandMapping(EX_CBJYQ_DJB regBook, VirtualPerson vp, ContractConcord concord, int srid, Dictionary<string, double> landareainfo)
        {
            List<ArcGisLandComplex> landCollection = new List<ArcGisLandComplex>();
            if (regBook.DJCBD == null || regBook.DJCBD.Count == 0)
            {
                return landCollection;
            }
            double concordTableArea = 0.0;
            double concordActualArea = 0.0;
            double concordAwareArea = 0.0;
            foreach (EX_CBJYQ_CBD cbd in regBook.DJCBD)
            {
                ArcGisLandComplex land = new ArcGisLandComplex();
                land.ContractorLand = SetAgriLandInformation(cbd);
                land.ContractorLand.OwnerId = vp.ID;
                land.ContractorLand.ConcordId = concord != null ? concord.ID : land.ContractorLand.ConcordId;
                if (cbd.Shape != null)
                {
                    land.ContractorLand.Shape = TransferGeometry(cbd.Shape, srid);
                }
                if (land.ContractorLand.TableArea != null)
                {
                    concordTableArea = concordTableArea + land.ContractorLand.TableArea.Value;
                    concordTableArea = ToolMath.RoundNumericFormat(concordTableArea, SystemSet.DecimalPlaces);
                }
                concordActualArea = concordActualArea + land.ContractorLand.ActualArea;
                concordActualArea = ToolMath.RoundNumericFormat(concordActualArea, SystemSet.DecimalPlaces);

                concordAwareArea = concordAwareArea + land.ContractorLand.AwareArea;
                concordAwareArea = ToolMath.RoundNumericFormat(concordAwareArea, SystemSet.DecimalPlaces);

                land.DotCollection = InitalizeDotInformation(cbd, srid);
                land.LineCollection = InitalizeLineInformation(cbd, srid);
                landCollection.Add(land);
            }
            landareainfo.Add("台账面积", concordTableArea);
            landareainfo.Add("实测面积", concordActualArea);
            landareainfo.Add("确权面积", concordAwareArea);
            InitalizeLandExpandInformtion(landCollection, regBook);
            return landCollection;
        }

        /// <summary>
        /// <see cref="ExGeometry"/> 将ExGeometry转换成Spatial.Geometry<see cref="IGeometry"/>。
        /// </summary>
        private YuLinTu.Spatial.Geometry TransferGeometry(GeometryWellKnownValue exchangeGeometry, int srid)
        {
            if (exchangeGeometry == null)
            {
                return null;
            }
            try
            {
                return YuLinTu.Spatial.Geometry.FromBytes(exchangeGeometry.WellKnownBinary, srid);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        private ContractLand SetAgriLandInformation(EX_CBJYQ_CBD exLand)
        {
            Zone zone = LandEntity.CurrentZone;
            if ((LandEntity.CurrentZone == null || LandEntity.CurrentZone.ID != exLand.SYQRID))
            {
                var zoneStation = DataInstance.CreateZoneWorkStation();
                Zone z = zoneStation.Get(exLand.SYQRID);
                if (z != null)
                    zone = z;
            }
            ContractLand land = new ContractLand();
            land.OwnerName = exLand.CBFMC;
            land.LandLevel = exLand.DJ;
            var landplattype = YuLinTu.Library.Entity.ePlantingType.Other;
            Enum.TryParse(exLand.ZZLX.ToString(), out landplattype);
            land.PlatType = ((int)(landplattype)).ToString();
            land.Name = exLand.DKMC;
            land.LandNumber = exLand.DKBM;

            land.CadastralNumber = land.LandNumber;
            land.LandCode = exLand.DL;
            land.TableArea = exLand.ELHTMJ;
            land.ExtendB = exLand.GTDJBH;
            land.ID = exLand.ID;
            land.ActualArea = ToolMath.RoundNumericFormat(exLand.SCMJ * 0.0015, SystemSet.DecimalPlaces);//实测面积处理 
            land.IsFarmerLand = exLand.SFJBNT;
            string[] arrayList = exLand.SZ.Split('#');
            land.NeighborEast = arrayList[0];
            land.NeighborSouth = arrayList[1];
            land.NeighborWest = arrayList[2];
            land.NeighborNorth = arrayList[3];
            land.PlantType = ((int)(ePlantProtectType)exLand.GBLX).ToString();
            land.ZoneCode = (string.IsNullOrEmpty(exLand.SZDY) && zone != null) ? zone.FullCode : exLand.SZDY;
            land.ZoneName = (string.IsNullOrEmpty(exLand.ZLDWMC) && zone != null) ? zone.FullName : exLand.ZLDWMC;
            land.SenderCode = (string.IsNullOrEmpty(exLand.ZLDWDM) && zone != null) ? zone.FullCode : exLand.ZLDWDM;
            if (exLand.ZLDWDM.Length == 16)
            {
                land.SenderCode = exLand.ZLDWDM.Substring(0, 12) + exLand.ZLDWDM.Substring(14, 2);
                land.ZoneCode = land.SenderCode;
            }
            land.SenderName = land.ZoneName;
            land.AwareArea = (exLand.QQMJ != null && exLand.QQMJ.HasValue) ? exLand.QQMJ.Value : 0.0;
            land.ManagementType = ((int)(eManageType)exLand.JYFS).ToString();
            land.IsFlyLand = exLand.SFFD;
            land.MotorizeLandArea = exLand.JDDMJ;
            land.LandName = exLand.DLBM;
            if (land.LandCode.IsNullOrEmpty() == false && land.LandName.IsNullOrEmpty())
            {
                land.LandName = LandUseTypeConvert.Instance.Convert(land.LandCode) as string;
            }
            var obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eLandPurposeType), exLand.TDYT);
            land.Purpose = ((int)(obj == null ? eLandPurposeType.Planting : (eLandPurposeType)(obj))).ToString();
            land.LandCategory = exLand.DKLB;
            land.OwnRightType = exLand.SYQXZ.IsNullOrEmpty() ? ((int)(eLandPropertyType.Collectived)).ToString() : ((int)(eLandPropertyType)Enum.Parse(typeof(eLandPropertyType), exLand.SYQXZ)).ToString();
            if (exLand.GBLX == ePlantProtectionType.Unknown)
            {
                land.PlantType = ((int)ePlantProtectType.UnKnown).ToString();
            }
            land.Status = ((int)eEntityStatus.End).ToString();
            land.ConstructMode = ((int)LandCategoryMapping.LandModeCodeMapping(exLand.QDFS)).ToString();
            if (land.ConstructMode.IsNullOrEmpty() || land.ConstructMode == "")
            {
                land.ConstructMode = ((int)eConstructMode.Family).ToString();
            }
            land.Comment = exLand.DKBZXX;
            if (land.SenderCode.Length == 16)
                land.SenderCode = land.SenderCode.Substring(0, 12) + land.SenderCode.Substring(14, 2);
            if (land.ZoneCode.Length == 16)
                land.ZoneCode = land.ZoneCode.Substring(0, 12) + land.ZoneCode.Substring(14, 2);
            InitalizeLandExpandInformtion(land, exLand);
            InitalizeAgriLandExpandInformation(land, exLand);
            return land;
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        /// <param name="land"></param>
        /// <param name="exLand"></param>
        private void InitalizeLandExpandInformtion(ContractLand land, EX_CBJYQ_CBD exLand)
        {
            AgricultureLandExpand landExpand = land.LandExpand;
            if (exLand.GCJZ != null && exLand.GCJZ.HasValue && exLand.GCJZ.Value > 0)
            {
                landExpand.Elevation = exLand.GCJZ;
            }
            landExpand.AgricultureNumber = exLand.DKBM;
            landExpand.ReferPerson = exLand.ZJRXM;//指界人
            landExpand.SurveyPerson = exLand.DCY;
            landExpand.SurveyDate = exLand.DCRQ;
            landExpand.SurveyChronicle = exLand.DCYJ;
            landExpand.CheckPerson = exLand.DCSHR;
            landExpand.CheckDate = exLand.DCSHRQ;
            landExpand.CheckOpinion = exLand.DCSHYJ;
            landExpand.ImageNumber = exLand.TFBH;
            landExpand.MeasureArea = exLand.SCMJ;
            land.LandExpand = landExpand;
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeAgriLandExpandInformation(ContractLand land, EX_CBJYQ_CBD exLand)
        {
            if (string.IsNullOrEmpty(exLand.LandCatalog))
            {
                return;
            }
            string[] val = exLand.LandCatalog.Split(new char[] { '|' });
            //if (val != null && val.Length > 0)
            //{
            //    object obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eLandCategoryType), val[0]);
            //    land.ConstructMode = ((int)(obj != null ? (YuLinTu.Library.Entity.eLandCategoryType)(obj) : YuLinTu.Library.Entity.eLandCategoryType.ContractLand)).ToString();
            //}
            land.PlotNumber = (val != null && val.Length > 1) ? val[1] : "";
            land.IsTransfer = (val != null && val.Length > 2) ? (val[2] == "1" ? true : false) : false;
            if (val != null && val.Length > 3)
            {
                object obj = Enum.Parse(typeof(YuLinTu.Library.Entity.eTransferType), (val[3] == null || val[3].Trim() == "") ? "Other" : val[3]);
                land.TransferType = ((int)(obj != null ? (YuLinTu.Library.Entity.eTransferType)(obj) : YuLinTu.Library.Entity.eTransferType.Other)).ToString();
            }
            if (val != null && val.Length > 4)
            {
                double area = 0.0;
                double.TryParse(val[4], out area);
                land.PertainToArea = area;
            }
            land.TransferTime = (val != null && val.Length > 5) ? val[5] : "";
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        /// <param name="landCollectio"></param>
        /// <param name="regBook"></param>
        private void InitalizeLandExpandInformtion(List<ArcGisLandComplex> landCollection, EX_CBJYQ_DJB regBook)
        {
            if (regBook.LYXZ == null)
            {
                return;
            }
            foreach (DJ_CBJYQ_LYXZ lyxz in regBook.LYXZ)
            {
                ArcGisLandComplex land = landCollection.Find(ld => ld.ContractorLand.ID == lyxz.DKID);
                if (land == null)
                {
                    continue;
                }
                var landExpand = land.ContractorLand.LandExpand;
                if (landExpand != null)
                {
                    landExpand.UseSituation = lyxz.LYQK;
                    landExpand.Yield = lyxz.CL > 0 ? lyxz.CL.ToString() : "";
                    landExpand.OutputValue = lyxz.CZ > 0 ? lyxz.CZ.ToString() : "";
                    landExpand.IncomeSituation = lyxz.SYQK;
                    land.ContractorLand.LandExpand = landExpand;
                }
            }
        }

        /// <summary>
        /// 初始化界址点信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> InitalizeDotInformation(EX_CBJYQ_CBD land, int srid)
        {
            List<DJ_CBJYQ_JZD_GEO> dotCollection = land.JZD;
            if (dotCollection == null || dotCollection.Count == 0)
            {
                return new List<BuildLandBoundaryAddressDot>();
            }
            List<BuildLandBoundaryAddressDot> jzds = new List<BuildLandBoundaryAddressDot>();
            foreach (DJ_CBJYQ_JZD_GEO jzd in dotCollection)
            {
                BuildLandBoundaryAddressDot dot = new BuildLandBoundaryAddressDot();
                dot.ID = jzd.ID;
                dot.LandID = jzd.DKID;
                dot.LandMarkType = ((int)(eLandMarkType)Enum.Parse(typeof(eLandMarkType), jzd.JBLX.IsNullOrEmpty() ? ((int)eLandMarkType.NoFlag).ToString() : jzd.JBLX)).ToString();
                dot.DotNumber = jzd.JZDH;
                dot.DotType = ((int)(eBoundaryPointType)Enum.Parse(typeof(eBoundaryPointType), jzd.JZDLX.IsNullOrEmpty() ? ((int)eBoundaryPointType.ResolvePoint).ToString() : jzd.JZDLX)).ToString();
                dot.ZoneCode = land.SZDY;
                dot.UniteDotNumber = jzd.TBJZDH;
                dot.LandNumber = land.DKBM;
                dot.LandType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
                dot.IsValid = true;//因为交换实体里面没有是否有效字段，所以都默认为有效
                //dot.Shape = jzd.Shape;               
                if (jzd.X != null && jzd.Y != null)
                {
                    Spatial.Coordinate coord = new Spatial.Coordinate((double)jzd.X, (double)jzd.Y);
                    dot.Shape = YuLinTu.Spatial.Geometry.CreatePoint(coord, srid);
                }
                jzds.Add(dot);
            }
            jzds.Sort((a, b) =>
            {
                var avalue = int.Parse(a.DotNumber.Substring(1));
                var bvalue = int.Parse(b.DotNumber.Substring(1));

                if (avalue >= bvalue)
                    return 1;
                else
                    return -1;
            });

            return jzds;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineInformation(EX_CBJYQ_CBD land, int srid)
        {
            List<DJ_CBJYQ_JZX_GEO> lineCollection = land.JZX;
            List<DJ_CBJYQ_JZD_GEO> pointCollection = land.JZD;
            if (lineCollection == null || lineCollection.Count == 0)
            {
                return new List<BuildLandBoundaryAddressCoil>();
            }
            List<BuildLandBoundaryAddressCoil> jzxs = new List<BuildLandBoundaryAddressCoil>();
            int index = 1;
            foreach (DJ_CBJYQ_JZX_GEO jzx in lineCollection)
            {
                BuildLandBoundaryAddressCoil line = new BuildLandBoundaryAddressCoil();
                line.ID = jzx.ID;
                Int32.TryParse(jzx.SXH, out index);
                line.OrderID = (short)index;
                line.LandID = jzx.DKID;
                line.StartPointID = jzx.SDID;
                line.EndPointID = jzx.ZDID;
                var Sptdot = pointCollection.Find(pt => pt.ID == jzx.SDID);
                var Eptdot = pointCollection.Find(pt => pt.ID == jzx.ZDID);
                if (Sptdot != null)
                {
                    line.StartNumber = Sptdot.JZDH;
                }
                if (Eptdot != null)
                {
                    line.EndNumber = Eptdot.JZDH;
                }
                line.LineType = ((int)(eBoundaryNatureType)Enum.Parse(typeof(eBoundaryNatureType), (jzx.JXXZ == null || jzx.JXXZ == "") ? ((int)eBoundaryNatureType.Other).ToString() : jzx.JXXZ)).ToString();
                line.CoilType = ((int)(eBoundaryLineCategory)Enum.Parse(typeof(eBoundaryLineCategory), (jzx.JZXLB == null || jzx.JZXLB == "") ? ((int)eBoundaryLineCategory.Other).ToString() : jzx.JZXLB)).ToString();
                line.Position = ((int)(eBoundaryLinePosition)Enum.Parse(typeof(eBoundaryLinePosition), (jzx.JZXWZ == null || jzx.JZXWZ == "") ? ((int)eBoundaryLinePosition.Left).ToString() : jzx.JZXWZ)).ToString();

                if (line.CoilType.StartsWith("0") == false)
                {
                    line.CoilType = "0" + line.CoilType;
                }
                if (line.Position.StartsWith("0") == false)
                {
                    line.Position = "0" + line.Position;
                }
                line.LandNumber = land.DKBM;
                line.Description = jzx.JZXSM;
                line.NeighborPerson = jzx.PLDWQLR;
                line.NeighborFefer = jzx.PLDWZJR;
                line.CoilLength = (jzx.JXCD != null && jzx.JXCD.HasValue) ? jzx.JXCD.Value : 0.0;
                line.ZoneCode = land.SZDY;
                line.LandType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
                if (jzx.Shape != null)
                {
                    jzx.Shape.Srid = srid;
                    line.Shape = jzx.Shape;
                }
                jzxs.Add(line);
            }
            return jzxs;
        }

        /// <summary>
        /// 发包方映射
        /// </summary>
        private CollectivityTissue AgricultureSenderMapping(EX_CBJYQ_DJB regBook)
        {
            if (regBook == null || regBook.FBF == null)
            {
                return null;
            }
            var tissueStation = DataInstance.CreateSenderWorkStation();
            CollectivityTissue tissue = LandEntity.Tissues.Find(ct => ct.ID == regBook.FBF.ID || ct.Code == regBook.FBF.BM || ct.Code == regBook.FBF.YBM);
            if (tissue == null)
            {
                tissue = tissueStation.Get(regBook.SZDY, regBook.FBFQC);
            }
            if (tissue == null && !string.IsNullOrEmpty(regBook.FBF.YBM))
            {
                tissue = tissueStation.Get(t => t.Code == regBook.FBF.YBM).FirstOrDefault();
            }
            if (tissue == null)
            {
                tissue = tissueStation.Get(t => t.Code == regBook.FBF.BM).FirstOrDefault();
            }
            if (tissue == null)
            {
                tissue = tissueStation.Get(regBook.FBF.ID);
            }
            if (tissue == null)
            {
                tissue = tissueStation.Get(regBook.SZDY.Substring(0, Zone.ZONE_VILLAGE_LENGTH), regBook.FBFQC);
            }
            if (tissue == null)
            {
                string zoneCode = regBook.SZDY.PadRight(14, '0');
                tissue = tissueStation.Get(t => t.Code == zoneCode).FirstOrDefault();
            }
            return tissue;
        }

        /// <summary>
        /// 初始化发包方信息
        /// </summary>
        private CollectivityTissue InitalzieTissueInformation(CollectivityTissue tissue, EX_CBJYQ_FBF fbf)
        {
            if (fbf == null)
            {
                return null;
            }
            bool addData = false;
            if (tissue == null)
            {
                tissue = new CollectivityTissue();
                addData = true;
            }
            tissue.Code = fbf.BM;
            tissue.SurveyChronicle = fbf.DCJS;
            tissue.SurveyDate = fbf.DCRQ;
            tissue.SurveyPerson = fbf.DCY;
            tissue.LawyerAddress = fbf.DZ;
            tissue.LawyerName = fbf.FZRXM;
            tissue.LawyerCartNumber = fbf.FZRZJHM;
            tissue.LawyerPosterNumber = fbf.YZBM;
            if (fbf.FZRZJLX != null)
            {
                object obj = Enum.Parse(typeof(eCredentialsType), fbf.FZRZJLX);
                tissue.LawyerCredentType = obj == null ? eCredentialsType.IdentifyCard : (eCredentialsType)obj;
            }
            tissue.LawyerTelephone = fbf.LXDH;
            tissue.CheckPerson = fbf.SHR;
            tissue.CheckDate = fbf.SHRQ;
            tissue.CheckOpinion = fbf.SHYJ;
            tissue.Name = fbf.MC;
            tissue.ZoneCode = fbf.SZDY;
            if (tissue.ZoneCode.Length == 16)
                tissue.ZoneCode = tissue.ZoneCode.Substring(0, 12) + tissue.ZoneCode.Substring(14, 2);
            if (addData)
            {
                var tissueStation = DataInstance.CreateSenderWorkStation();
                tissueStation.Add(tissue);
            }

            return tissue;
        }

        /// <summary>
        /// 转换集体经济组织类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private eTissueType TransformTissueType(YuLinTu.PropertyRight.eCollectivityTissueType type)
        {
            switch (type)
            {
                case YuLinTu.PropertyRight.eCollectivityTissueType.General:
                    return eTissueType.General;
                case YuLinTu.PropertyRight.eCollectivityTissueType.NewKind:
                    return eTissueType.NewKind;
                default:
                    return eTissueType.UnKnown;
            }
        }

        /// <summary>
        /// 合同映射
        /// </summary>
        private ContractConcord AgricultureConcordMapping(EX_CBJYQ_DJB regBook)
        {
            if (string.IsNullOrEmpty(regBook.HTBH))
            {
                return null;
            }
            ContractConcord concord = LandEntity.Concords.Find(cd => cd.ID == regBook.ID);
            if (concord != null)
            {
                return concord;
            }
            concord = new ContractConcord();
            concord.ContracerType = ((int)(eContractorType)Enum.Parse(typeof(YuLinTu.Library.Entity.eContractorType), regBook.CBFLX)).ToString();
            concord.ContractDate = regBook.CBHTQDRQ;
            concord.ArableLandStartTime = regBook.CBKSRQ;
            concord.ArableLandEndTime = regBook.CBJSRQ;
            concord.ArableLandType = ((int)LandCategoryMapping.LandModeCodeMapping(regBook.CBFS)).ToString();
            concord.Flag = regBook.CBJSRQ == null;// 承包期限为长久
            concord.SenderName = regBook.FBFQC;
            concord.ConcordNumber = string.IsNullOrEmpty(regBook.YHTBH) ? regBook.HTBH : regBook.YHTBH;
            concord.ID = regBook.ID;
            concord.LandPurpose = ((int)(eLandPurposeType)Enum.Parse(typeof(YuLinTu.Library.Entity.eLandPurposeType), regBook.TDCBYT)).ToString();
            concord.ZoneCode = regBook.SZDY;
            if (concord.Flag)
            {
                concord.ManagementTime = "长久";
            }
            else
            {
                concord.ManagementTime = ToolDateTime.CalcateTerm(concord.ArableLandStartTime, concord.ArableLandEndTime); ;
            }
            concord.ContracterName = regBook.CBFMC;
            concord.ContracterId = regBook.CBFID;
            concord.ContracterIdentifyNumber = regBook.CBFZJHM;
            concord.TotalTableArea = (regBook.CBDELHTZMJ != null && regBook.CBDELHTZMJ.HasValue) ? regBook.CBDELHTZMJ.Value : 0.0;
            concord.CountAwareArea = (regBook.QQZMJ != null && regBook.QQZMJ.HasValue) ? regBook.QQZMJ.Value : 0.0;
            concord.CountActualArea = ToolMath.RoundNumericFormat(regBook.CBDSCZMJ * 0.0015, SystemSet.DecimalPlaces);
            concord.PublicityChronicle = regBook.GSJS;
            concord.PublicityChroniclePerson = regBook.GSJSR;
            concord.PublicityDate = regBook.GSJSRQ;
            concord.PublicityContractor = regBook.CBFGSJGQRR;
            concord.PublicityResultDate = regBook.CBFGSJGQRRQ;
            concord.PublicityResult = regBook.CBFGSJGQRYJ;
            concord.PublicityCheckPerson = regBook.GSSHR;
            concord.PublicityCheckDate = regBook.GSSHRQ;
            concord.PublicityCheckOpinion = regBook.GSSHYJ;
            concord.IsValid = true;
            return concord;
        }

        /// <summary>
        /// 申请书映射
        /// </summary>
        private ContractRequireTable AgricultureRequirBookMapping(EX_CBJYQ_DJB regBook)
        {
            if (regBook.DJSQS == null)
            {
                return null;
            }
            Guid requireId = regBook.DJSQS.ID;
            ContractRequireTable requireTable = LandEntity.Tables.Find(rt => rt.ID == requireId);
            if (requireTable == null)
            {
                var requireStation = DataInstance.CreateRequireTableWorkStation();
                requireTable = requireStation.Get(requireId);
            }
            if (requireTable != null)
            {
                return null;
            }
            requireTable = new ContractRequireTable();
            requireTable.ID = regBook.DJSQS.DJBID;
            requireTable.Number = regBook.DJSQS.BH;
            requireTable.Year = regBook.DJSQS.SQRQ.Year.ToString();
            requireTable.Date = regBook.DJSQS.SQRQ;
            requireTable.ZoneCode = regBook.SZDY;
            return requireTable;
        }

        /// <summary>
        /// 权证映射
        /// </summary>
        private ContractRegeditBook AgricultureWarrantMapping(EX_CBJYQ_DJB regBook)
        {
            if (regBook.QZ == null)
            {
                return null;
            }
            ContractRegeditBook book = LandEntity.Books.Find(bk => bk.ID == regBook.QZ.DJBID);
            if (book == null)
            {
                var bookStation = DataInstance.CreateRegeditBookStation();
                book = bookStation.Get(regBook.QZ.DJBID);
            }
            if (book != null)
            {
                book.ZoneCode = regBook.SZDY;
                return book;
            }
            book = new ContractRegeditBook();
            book.ID = regBook.QZ.DJBID;
            book.WriteDate = regBook.QZ.DZRQ;
            book.WriteOrganization = regBook.QZ.FZJG;
            book.SendOrganization = regBook.QZ.FZJGJC;
            book.SendDate = regBook.QZ.FZRQ;
            book.Number = regBook.QZBH;
            book.Year = regBook.QZ.QZNH;
            book.RegeditNumber = string.IsNullOrEmpty(regBook.YQZBH) ? regBook.QZBH : regBook.YQZBH;
            book.ZoneCode = regBook.SZDY;
            //陈泽林 20161227
            book.SerialNumber = regBook.QZ.QZDZH;
            book.ContractRegeditBookExcursus = regBook.BZ;
            return book;
        }

        #endregion
    }
}
