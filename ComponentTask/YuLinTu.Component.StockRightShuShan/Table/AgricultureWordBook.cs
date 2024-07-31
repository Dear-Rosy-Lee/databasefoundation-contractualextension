using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.Table
{
    public class AgricultureWordBook : Library.Business.AgricultureWordBook
    {
        public List<ContractAccountLandFamily> AccountLandFamily { get; set; }

        public List<ContractLand> LandCollectionAll { get; set; }

        internal List<ExportGroupEntity> GroupEntitys { get; set; }

        protected List<int> _titleInRowList = new List<int>(); //记录标题所在行

        public List<Dictionary> DLDJ
        {
            get { return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ); }
        }

        public List<Dictionary> TDYT
        {
            get { return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT); }
        }

        public List<Dictionary> ZZLX
        {
            get { return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.ZZLX); }
        }

        public List<Dictionary> TDLYLX
        {
            get { return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX); }
        }


        public List<Dictionary> DKLB
        {
            get { return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB); }
        }

        /// <summary>
        /// 当前地域下权利人集合
        /// </summary>
        public List<VirtualPerson> Contractors { get; set; }


        protected override bool OnSetParamValue(object data)
        {
            try
            {
                base.OnSetParamValue(data);

                WriteParcelInformation();

                SetBookmarkValue("ConcordNumberER", Contractor?.FamilyExpand?.ConcordNumber);
                SetBookmarkValue("BookNumberER", Contractor?.FamilyExpand?.WarrantNumber);

            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        #region Override - Methods

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected override void InitialEntity(object data)
        {
            if (data == null)
            {
                return;
            }
            if (data is VirtualPerson)
            {
                Contractor = data as VirtualPerson;
            }
            if (data is Zone)
            {
                CurrentZone = data as Zone;
            }
            if (data is CollectivityTissue)
            {
                Tissue = data as CollectivityTissue;
            }
            if (data is ContractConcord)
            {
                Concord = data as ContractConcord;
            }
            if (data is YuLinTu.Library.Entity.ContractRegeditBook)
            {
                Book = data as YuLinTu.Library.Entity.ContractRegeditBook;
            }
            if (data is List<ContractLand>)
            {
                LandCollection = data as List<ContractLand>;
            }
        }

        /// <summary>
        /// 填写行政区域信息
        /// </summary>
        protected override void WriteZoneInformation()
        {
            if (CurrentZone == null)
            {
                return;
            }
            string countyCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = CurrentZone;
            string unitName = county != null ? CurrentZone.FullName.Replace(county.FullName, "") : CurrentZone.FullName;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ZoneName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMark.LocationName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMark.TownUnitName + (i == 0 ? "" : i.ToString()), unitName);
                SetBookmarkValue(AgricultureBookMark.CountyUnitName + (i == 0 ? "" : i.ToString()),
                    county != null ? (county.Name + unitName) : unitName);
            }
            string zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_COUNTY_LENGTH);



            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.CountyName + (i == 0 ? "" : i.ToString()), zoneName);
                // SetBookmarkValue("County1" + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallCountyName + (i == 0 ? "" : i.ToString()),
                    !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            var country =
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_PROVICE_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ProviceName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallProviceName + (i == 0 ? "" : i.ToString()),
                    !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");

            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_CITY_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.CityName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallCityName + (i == 0 ? "" : i.ToString()),
                    !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_TOWN_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.TownName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallTownName + (i == 0 ? "" : i.ToString()),
                    !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            if (CurrentZone.Level >= eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_VILLAGE_LENGTH);
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMark.VillageName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMark.SmallVillageName + (i == 0 ? "" : i.ToString()),
                        !string.IsNullOrEmpty(zoneName)
                            ? zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", "")
                            : "");
                    SetBookmarkValue(AgricultureBookMark.VillageUnitName + (i == 0 ? "" : i.ToString()),
                        zoneName + CurrentZone.Name);
                }
            }
            if (CurrentZone.Level == eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_GROUP_LENGTH);
                string number = ToolString.GetLeftNumberWithInString(zoneName);
                string groupName = string.IsNullOrEmpty(number)
                    ? zoneName
                    : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
                string smallGroup = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMark.GroupName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMark.ChineseGroupName + (i == 0 ? "" : i.ToString()), groupName);
                    SetBookmarkValue(AgricultureBookMark.SmallChineseGroupName + (i == 0 ? "" : i.ToString()),
                        smallGroup);
                    SetBookmarkValue(AgricultureBookMark.SmallGroupName + (i == 0 ? "" : i.ToString()),
                        !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
                }
            }
        }

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        protected override void WriteContractorInformaion()
        {
            if (Contractor == null)
            {
                return;
            }
            string townName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            int familyNumber = 0;
            if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
            {
                Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
            }
            string familyString = familyNumber > 0 ? string.Format("{0:D4}", familyNumber) : "";
            string familyAllString = Contractor.ZoneCode;
            familyAllString = familyAllString.PadRight(14, '0') + familyString;
            if (AgricultureSetting.AgricultureLandWordFamilyNumber)
            {
                familyString = familyAllString;
            }
            VirtualPersonExpand expand = Contractor.FamilyExpand;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorName + (i == 0 ? "" : i.ToString()),
                    InitalizeFamilyName(Contractor.Name)); //承包方姓名
                SetBookmarkValue(AgricultureBookMark.ContractorNumber + (i == 0 ? "" : i.ToString()), familyString);
                //承包方户号
                SetBookmarkValue(AgricultureBookMark.ContractorAllNumber + (i == 0 ? "" : i.ToString()), familyAllString);
                //承包方全户号
                SetBookmarkValue(AgricultureBookMark.ContractorNumber + (i == 0 ? "" : i.ToString()), familyAllString.GetLastString(4));
                //承包方全户号
                if (Contractor.CardType == eCredentialsType.IdentifyCard)
                {
                    SetBookmarkValue(AgricultureBookMark.ContractorIdentifyNumber + (i == 0 ? "" : i.ToString()),
                        Contractor.Number); //承包方身份证号码
                }
                else
                {
                    SetBookmarkValue(AgricultureBookMark.ContractorOtherCardNumber + (i == 0 ? "" : i.ToString()),
                        Contractor.Number); //其他证件号码
                }
                SetBookmarkValue(AgricultureBookMark.ContractorAllocationPerson + (i == 0 ? "" : i.ToString()),
                    expand.AllocationPerson); //承包方实际分配人数
                SetBookmarkValue(AgricultureBookMark.ContractorComment + (i == 0 ? "" : i.ToString()),
                    Contractor.Comment); //承包方备注
                //SetBookmarkValue(AgricultureBookMark.ContractorLocation + (i == 0 ? "" : i.ToString()),
                //    Contractor.Address); //坐落地域
                //SetBookmarkValue(AgricultureBookMark.ContractorCommunicate + (i == 0 ? "" : i.ToString()),
                //    Contractor.Address); //通信地址
                SetBookmarkValue(AgricultureBookMark.ContractorTelephone + (i == 0 ? "" : i.ToString()),
                    Contractor.Telephone); //承包方电话号码
                SetBookmarkValue(AgricultureBookMark.ContractorPostNumber + (i == 0 ? "" : i.ToString()),
                    Contractor.PostalNumber); //承包方邮政编码
                SetBookmarkValue(AgricultureBookMark.ContractorAddress + (i == 0 ? "" : i.ToString()),
                    Contractor.Address); //承包方地址
                SetBookmarkValue(AgricultureBookMark.ContractorAddressTown + (i == 0 ? "" : i.ToString()), townName);
                //承包方地址到镇
                SetBookmarkValue(AgricultureBookMark.ContractorAddressVillage + (i == 0 ? "" : i.ToString()),
                    villageName); //承包方地址到村
                SetBookmarkValue(AgricultureBookMark.ContractorAddressGroup + (i == 0 ? "" : i.ToString()), groupName);
                //承包方地址到组
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyPerson + (i == 0 ? "" : i.ToString()),
                    expand.SurveyPerson); //承包方调查员
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyDate + (i == 0 ? "" : i.ToString()),
                    (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                        ? ToolDateTime.GetLongDateString(expand.SurveyDate.Value)
                        : ""); //承包方调查员
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyChronicle + (i == 0 ? "" : i.ToString()),
                    expand.SurveyChronicle); //承包方调查记事
                SetBookmarkValue(AgricultureBookMark.ContractorCheckPerson + (i == 0 ? "" : i.ToString()),
                    expand.CheckPerson); //承包方审核员
                SetBookmarkValue(AgricultureBookMark.ContractorCheckDate + (i == 0 ? "" : i.ToString()),
                    (expand.CheckDate != null && expand.CheckDate.HasValue)
                        ? ToolDateTime.GetLongDateString(expand.CheckDate.Value)
                        : ""); //承包方审核日期
                SetBookmarkValue(AgricultureBookMark.ContractorCheckOpinion + (i == 0 ? "" : i.ToString()),
                    expand.CheckOpinion); //承包方审核意见
                SetBookmarkValue("ContractorTimeLimit", "");
            }
            WriteCredentialsInformation();
            WriteSharePersonInformation();
            if (expand.ConcordStartTime != null && expand.ConcordEndTime != null)
                SetBookmarkValue("ContractDueTime", expand.ConcordStartTime?.ToString("yyyy年MM月dd日") + "至" + expand.ConcordEndTime?.ToString("yyyy年MM月dd日"));
        }

        /// <summary>
        /// 设置证件类型
        /// </summary>
        protected override void WriteCredentialsInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            eCredentialsType type = Contractor.CardType;
            switch (type)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard, "R"); //证件号码
                    break;
                case eCredentialsType.AgentCard:
                    SetBookmarkValue(AgricultureBookMark.AgentCard, "R"); //证件号码
                    break;
                case eCredentialsType.OfficerCard:
                    SetBookmarkValue(AgricultureBookMark.OfficerCard, "R"); //证件号码
                    break;
                case eCredentialsType.Other:
                    SetBookmarkValue(AgricultureBookMark.CredentialOther, "R"); //证件号码
                    break;
                case eCredentialsType.Passport:
                    SetBookmarkValue(AgricultureBookMark.Passport, "R"); //证件号码
                    break;
                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet, "R"); //证件号码
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        protected override void WriteLandInformation()
        {
            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }
            int index = 1;
            LandCollection = SortLandCollection(LandCollection);
            foreach (var land in LandCollection)
            {
                AgricultureLandExpand expand = land.LandExpand;
                SetBookmarkValue(AgricultureBookMark.AgricultureName + index, land.Name); //地块名称
                string landNumber = land.LandNumber;
                SetBookmarkValue(AgricultureBookMark.AgricultureNumber + index, landNumber); //地块编码
                string actualAreaString = land.ActualArea > 0.0
                    ? ToolMath.SetNumbericFormat(InitalizeArea(land.ActualArea).ToString(), 2)
                    : "";
                SetBookmarkValue(AgricultureBookMark.AgricultureActualArea + index, land.ActualArea.AreaFormat()); //实测面积
                string awareAreaString = land.AwareArea > 0.0
                    ? ToolMath.SetNumbericFormat(InitalizeArea(land.AwareArea).ToString(), 2)
                    : "";
                SetBookmarkValue(AgricultureBookMark.AgricultureAwareArea + index, land.AwareArea.AreaFormat()); //确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureTableArea + index, land.TableArea.AreaFormat()); //台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureModoArea + index, land.MotorizeLandArea.AreaFormat()); //地块机动地面积
                InitalizeSmallNumber(index, ContractLand.GetLandNumber(land.CadastralNumber));
                var level = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DLDJ && d.Code == land.LandLevel);
                string levelString = level != null ? level.Name : "";
                levelString = levelString == "未知" ? "" : levelString;
                levelString = AgricultureSetting.UseSystemLandLevelDescription
                    ? levelString
                    : InitalizeLandLevel(land.LandLevel);
                SetBookmarkValue(AgricultureBookMark.AgricultureLandLevel + index, levelString); //等级
                string landName = !string.IsNullOrEmpty(land.LandName) ? land.LandName : "";
                if (string.IsNullOrEmpty(landName) && DictList != null)
                {
                    Dictionary lt = DictList.Find(ld => ld.Code == land.LandCode);
                    landName = lt != null ? lt.Name : "";
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureLandType + index, landName == "未知" ? "" : landName);
                //地类
                SetBookmarkValue(AgricultureBookMark.AgricultureIsFarmarLand + index,
                    (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue)
                        ? ""
                        : (land.IsFarmerLand.Value ? "是" : "否")); //是否基本农田
                SetBookmarkValue(AgricultureBookMark.AgricultureEast + index, land.NeighborEast); //东
                SetBookmarkValue(AgricultureBookMark.AgricultureEastName + index, "东:" + land.NeighborEast); //东
                SetBookmarkValue(AgricultureBookMark.AgricultureSouth + index, land.NeighborSouth); //南
                SetBookmarkValue(AgricultureBookMark.AgricultureSouthName + index, "南:" + land.NeighborSouth); //南
                SetBookmarkValue(AgricultureBookMark.AgricultureWest + index, land.NeighborWest); //西
                SetBookmarkValue(AgricultureBookMark.AgricultureWestName + index, "西:" + land.NeighborWest); //西
                SetBookmarkValue(AgricultureBookMark.AgricultureNorth + index, land.NeighborNorth); //北
                SetBookmarkValue(AgricultureBookMark.AgricultureNorthName + index, "北:" + land.NeighborNorth); //北
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighbor + index,
                    SystemSet.NergionbourSet ? InitalizeLandNeightor(land) : "见附图"); //四至
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighborFigure + index, "见附图"); //四至见附图
                SetBookmarkValue(AgricultureBookMark.AgricultureComment + index, land.Comment); //地块备注
                var mode =
                    DictList.Find(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS && d.Code == land.ConstructMode);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstructMode + index, mode != null ? mode.Name : "");
                //承包方式
                var callog = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DKLB && d.Code == land.LandCategory);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstractType + index, callog != null ? callog.Name : "");
                //地块类别
                SetBookmarkValue(AgricultureBookMark.AgriculturePlotNumber + index, land.PlotNumber); //地块畦数
                var manager = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.ManagementType);
                SetBookmarkValue(AgricultureBookMark.AgricultureManagerType + index, manager != null ? manager.Name : "");
                //地块经营方式
                SetBookmarkValue(AgricultureBookMark.AgricultureSourceFamilyName + index, land.FormerPerson); //原户主姓名
                var plant = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.GBZL && d.Code == land.PlantType);
                string plantType = plant != null ? plant.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlantType + index, plantType == "未知" ? "" : plantType);
                //耕保类型
                if (land.IsTransfer)
                {
                    var transMode =
                        DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.TransferType);
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterMode + index,
                        transMode != null ? transMode.Name : ""); //流转方式
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterTerm + index, land.TransferTime); //流转期限
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterArea + index,
                        land.PertainToArea > 0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : "");
                    //流转面积
                }
                var plat = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.ZZLX && d.Code == land.PlatType);
                string platType = plat != null ? plat.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlatType + index, platType == "未知" ? "" : platType);
                //种植类型
                var purpose = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == land.Purpose);
                string landPurpose = purpose != null ? purpose.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePurpose + index,
                    (string.IsNullOrEmpty(landPurpose) || ToolMath.MatchEntiretyNumber(landPurpose))
                        ? "种植业"
                        : landPurpose); //土地用途
                SetBookmarkValue(AgricultureBookMark.AgricultureUseSituation + index, expand.UseSituation); //土地利用情况
                SetBookmarkValue(AgricultureBookMark.AgricultureYield + index, expand.Yield); //土地产量情况
                SetBookmarkValue(AgricultureBookMark.AgricultureOutputValue + index, expand.OutputValue); //土地产值情况
                SetBookmarkValue(AgricultureBookMark.AgricultureIncomeSituation + index, expand.IncomeSituation);
                //土地收益情况
                SetBookmarkValue(AgricultureBookMark.AgricultureElevation + index, expand.Elevation.ToString()); //高程
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyPerson + index, expand.SurveyPerson); //地块调查员
                if (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureSurveyDate + index,
                        ToolDateTime.GetLongDateString(expand.SurveyDate.Value)); //地块调查日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyChronicle + index, expand.SurveyChronicle);
                //地块调查记事
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckPerson + index, expand.CheckPerson); //地块审核员
                if (expand.CheckDate != null && expand.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureCheckDate + index,
                        ToolDateTime.GetLongDateString(expand.CheckDate.Value)); //地块审核日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckOpinion + index, expand.CheckOpinion); //地块审核意见
                SetBookmarkValue(AgricultureBookMark.AgricultureImageNumber + index, expand.ImageNumber); //地块图幅号
                SetBookmarkValue(AgricultureBookMark.AgricultureFefer + index, expand.ReferPerson); //地块指界人
                index++;
            }
            WriteLandCalInformation();
            WriteReclamationInformation();
        }

        /// <summary>
        /// 书写发包方信息
        /// </summary>
        protected override void WriteSenderInformation()
        {
            if (Tissue == null)
            {
                return;
            }
            string senderNameExpress = InitalizeSenderExpress(); //发包方名称扩展如(第一村民小组)。
            string townName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            var layerCard =
                DictList.Find(
                    d => d.GroupCode == DictionaryTypeInfo.ZJLX && d.Code == ((int)Tissue.LawyerCredentType).ToString());
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.SenderName + (i == 0 ? "" : i.ToString()), Tissue.Name); //发包方名称
                SetBookmarkValue(AgricultureBookMark.SenderNameExpress + (i == 0 ? "" : i.ToString()), senderNameExpress);
                //发包方名称扩展如(第一村民小组)
                SetBookmarkValue(AgricultureBookMark.SenderLawyerName + (i == 0 ? "" : i.ToString()), Tissue.LawyerName);
                //发包方法人名称
                SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone + (i == 0 ? "" : i.ToString()),
                    Tissue.LawyerTelephone); //发包方法人联系方式
                SetBookmarkValue(AgricultureBookMark.SenderLawyerAddress + (i == 0 ? "" : i.ToString()),
                    Tissue.LawyerAddress); //发包方法人地址
                SetBookmarkValue(AgricultureBookMark.SenderLawyerPostNumber + (i == 0 ? "" : i.ToString()),
                    Tissue.LawyerPosterNumber); //发包方法人邮政编码
                SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentType + (i == 0 ? "" : i.ToString()),
                    layerCard != null ? layerCard.Name : ""); //发包方法人证件类型
                SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber + (i == 0 ? "" : i.ToString()),
                    Tissue.LawyerCartNumber); //发包方法人证件号码
                SetBookmarkValue(AgricultureBookMark.SenderCode + (i == 0 ? "" : i.ToString()), Tissue.Code); //发包方代码
                SetBookmarkValue(AgricultureBookMark.SenderTownName + (i == 0 ? "" : i.ToString()), townName); //发包方到镇
                SetBookmarkValue(AgricultureBookMark.SenderVillageName + (i == 0 ? "" : i.ToString()), villageName);
                //发包方到村
                SetBookmarkValue(AgricultureBookMark.SenderGroupName + (i == 0 ? "" : i.ToString()), groupName); //发包方到组
                SetBookmarkValue(AgricultureBookMark.SenderSurveyChronicle + (i == 0 ? "" : i.ToString()),
                    Tissue.SurveyChronicle); //调查记事
                SetBookmarkValue(AgricultureBookMark.SenderSurveyPerson + (i == 0 ? "" : i.ToString()),
                    Tissue.SurveyPerson); //调查员
                if (Tissue.SurveyDate != null && Tissue.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.SenderSurveyDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Tissue.SurveyDate.Value)); //调查日期
                }
                SetBookmarkValue(AgricultureBookMark.SenderCheckPerson + (i == 0 ? "" : i.ToString()),
                    Tissue.CheckPerson); //审核员
                if (Tissue.CheckDate != null && Tissue.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.SenderCheckDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Tissue.CheckDate.Value)); //审核日期
                }
                SetBookmarkValue(AgricultureBookMark.SenderChenkOpinion + (i == 0 ? "" : i.ToString()),
                    Tissue.CheckOpinion); //审核意见
            }
        }

        /// <summary>
        /// 填写合同信息
        /// </summary>
        protected override void WriteConcordInformation()
        {
            if (Concord == null || string.IsNullOrEmpty(Concord.ConcordNumber))
            {
                return;
            }
            string townName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            int landCount = 0;
            if (landCount == 0 && LandCollection != null)
            {
                landCount = LandCollection.Count(ld => ld.ConcordId == Concord.ID);
            }
            string landPurpose =
                DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == Concord.LandPurpose)?.Name;
            if (!string.IsNullOrEmpty(landPurpose) && ToolMath.MatchEntiretyNumber(landPurpose))
            {
                landPurpose = "种植业";
            }
            var dic = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS && d.Code == Concord.ArableLandType);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ConcordNumber + (i == 0 ? "" : i.ToString()), Concord.ConcordNumber);
                //合同编号
                SetBookmarkValue(AgricultureBookMark.ConcordTrem + (i == 0 ? "" : i.ToString()), Concord.ManagementTime);
                //合同期限
                SetBookmarkValue(AgricultureBookMark.ConcordMode + (i == 0 ? "" : i.ToString()),
                    dic != null ? dic.Name : ""); //合同承包方式
                SetBookmarkValue(AgricultureBookMark.ConcordPurpose + (i == 0 ? "" : i.ToString()), landPurpose);
                //合同土地用途
                SetBookmarkValue(AgricultureBookMark.ConcordLandCount + (i == 0 ? "" : i.ToString()),
                    landCount.ToString()); //合同中地块总数
                SetBookmarkValue(AgricultureBookMark.ConcordActualAreaCount + (i == 0 ? "" : i.ToString()),
                    Concord.CountActualArea.AreaFormat()); //合同总实测面积
                SetBookmarkValue(AgricultureBookMark.ConcordAwareAreaCount + (i == 0 ? "" : i.ToString()),
                    Concord.CountAwareArea.AreaFormat()); //合同总确权面积
                SetBookmarkValue(AgricultureBookMark.ConcordTableAreaCount + (i == 0 ? "" : i.ToString()),
                    Concord.TotalTableArea.AreaFormat()); //合同块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.ConcordModoAreaCount + (i == 0 ? "" : i.ToString()),
                    Concord.CountMotorizeLandArea.AreaFormat()); //合同总机动地面积
                SetBookmarkValue(AgricultureBookMark.ConcordAddress + (i == 0 ? "" : i.ToString()),
                    Concord.SecondContracterLocated); //合同中承包方地址
            }
            WriteConcordModeInformation();
            WriteConcordStartAndEndTime();
        }

        /// <summary>
        /// 书写权证信息
        /// </summary>
        protected override void WriteBookInformation()
        {
            if (Book == null || string.IsNullOrEmpty(Book.RegeditNumber))
            {
                return;
            }
            //string number = (Book != null && AgriculturePrintSetting.BookNumberPrintMedian > 0 && AgriculturePrintSetting.BookNumberPrintMedian <= Book.RegeditNumber.Length) ? Book.RegeditNumber.Substring(0, AgriculturePrintSetting.BookNumberPrintMedian) : Book.RegeditNumber.Substring(0, Book.RegeditNumber.Length - 1);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookNumber + (i == 0 ? "" : i.ToString()), Book.Number); //编号
                SetBookmarkValue(AgricultureBookMark.BookOrganName + (i == 0 ? "" : i.ToString()), Book.SendOrganization);
                SetBookmarkValue("BookOrganName2" + (i == 0 ? "" : i.ToString()), Book.SendOrganization);
                //发证机关名称
                SetBookmarkValue(AgricultureBookMark.BookYear + (i == 0 ? "" : i.ToString()), Book.Year); //年号
                SetBookmarkValue(AgricultureBookMark.BookWarrantNumber + (i == 0 ? "" : i.ToString()), Book.Number);
                //权证编号
                SetBookmarkValue(AgricultureBookMark.BookAllNumber + (i == 0 ? "" : i.ToString()),
                    Book.SendOrganization + "农地承包权(" + Book.Year + ")第" + Book.RegeditNumber + "号");
                SetBookmarkValue(AgricultureBookMark.BookSerialNumber + (i == 0 ? "" : i.ToString()),
                    Book.Number); //六位流水号
                SetBookmarkValue(AgricultureBookMark.BookFullSerialNumber + (i == 0 ? "" : i.ToString()),
                    Book.SendOrganization + "农地承包权(" + Book.Year + ")第" + Book.RegeditNumber + "号");
                //所有权证编号(包括发证机关、年号、流水号)
            }
            WriteAwareDateInformation();
            WriteSenderDateInformation();
        }

        /// <summary>
        /// 填写颁证日期信息
        /// </summary>
        protected override void WriteAwareDateInformation()
        {
            string year = Book.PrintDate != null ? Book.PrintDate.Year.ToString() : "";
            string awareYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.ToString()) : year;
            string oneYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : "";
            string twoYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : "";
            string month = Book.PrintDate != null ? Book.PrintDate.Month.ToString() : "";
            string awareMonth = !string.IsNullOrEmpty(month) ? ToolMath.GetChineseLowNumber(month.ToString()) : month;
            if (awareMonth.Equals("一十"))
            {
                awareMonth = "十";
            }
            string day = Book.PrintDate != null ? Book.PrintDate.Day.ToString() : "";
            string awareday = !string.IsNullOrEmpty(day) ? ToolMath.GetChineseLowNumber(day.ToString()) : day;
            if (awareday.Equals("一十"))
            {
                awareday = "十";
            }
            string allAwareString = year + "年" + month + "月" + day + "日";
            string shortYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? year.Substring(2) : year;
            string firstYear = (!string.IsNullOrEmpty(year) && year.Length > 0)
                ? ToolMath.GetChineseLowNimeric(year.Substring(0, 1))
                : year;
            string secondYear = (!string.IsNullOrEmpty(year) && year.Length > 1)
                ? ToolMath.GetChineseLowNimeric(year.Substring(1, 1))
                : year;
            string threeYear = (!string.IsNullOrEmpty(year) && year.Length > 2)
                ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1))
                : year;
            string fourYear = (!string.IsNullOrEmpty(year) && year.Length > 3)
                ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1))
                : year;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookAwareYear + (i == 0 ? "" : i.ToString()), awareYear); //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareFirstYear + (i == 0 ? "" : i.ToString()), firstYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareSecondYear + (i == 0 ? "" : i.ToString()), secondYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareThreeYear + (i == 0 ? "" : i.ToString()), threeYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareFourYear + (i == 0 ? "" : i.ToString()), fourYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareShortYear + (i == 0 ? "" : i.ToString()), shortYear);
                //打印日期到年后2位
                SetBookmarkValue(AgricultureBookMark.BookAwareOneYear + (i == 0 ? "" : i.ToString()), oneYear);
                //打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMark.BookAwareLastYear + (i == 0 ? "" : i.ToString()), twoYear);
                //打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMark.BookAwareMonth + (i == 0 ? "" : i.ToString()), awareMonth);
                //打印日期到月
                SetBookmarkValue(AgricultureBookMark.BookAwareDay + (i == 0 ? "" : i.ToString()), awareday); //打印日期到日
                SetBookmarkValue(AgricultureBookMark.BookAllAwareDate + (i == 0 ? "" : i.ToString()), allAwareString);
                //打印所有颁证日期
            }
        }

        /// <summary>
        /// 填写填证日期信息
        /// </summary>
        protected override void WriteSenderDateInformation()
        {
            string year = Book.WriteDate != null ? Book.WriteDate.Year.ToString() : "";
            string awareYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.ToString()) : year;
            string oneYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : "";
            string twoYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : "";
            string month = Book.WriteDate != null ? Book.WriteDate.Month.ToString() : "";
            string awareMonth = !string.IsNullOrEmpty(month) ? ToolMath.GetChineseLowNumber(month.ToString()) : month;
            if (awareMonth.Equals("一十"))
            {
                awareMonth = "十";
            }
            string day = Book.WriteDate != null ? Book.WriteDate.Day.ToString() : "";
            string awareday = !string.IsNullOrEmpty(day) ? ToolMath.GetChineseLowNumber(day.ToString()) : day;
            if (awareday.Equals("一十"))
            {
                awareday = "十";
            }
            string allAwareString = year + "年" + month + "月" + day + "日";
            string shortYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? year.Substring(2) : year;
            string firstYear = (!string.IsNullOrEmpty(year) && year.Length > 0)
                ? ToolMath.GetChineseLowNimeric(year.Substring(0, 1))
                : year;
            string secondYear = (!string.IsNullOrEmpty(year) && year.Length > 1)
                ? ToolMath.GetChineseLowNimeric(year.Substring(1, 1))
                : year;
            string threeYear = (!string.IsNullOrEmpty(year) && year.Length > 2)
                ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1))
                : year;
            string fourYear = (!string.IsNullOrEmpty(year) && year.Length > 3)
                ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1))
                : year;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookWriteYear + (i == 0 ? "" : i.ToString()), awareYear); //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteFirstYear + (i == 0 ? "" : i.ToString()), firstYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteSecondYear + (i == 0 ? "" : i.ToString()), secondYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteThreeYear + (i == 0 ? "" : i.ToString()), threeYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteFourYear + (i == 0 ? "" : i.ToString()), fourYear);
                //打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteShortYear + (i == 0 ? "" : i.ToString()), shortYear);
                //打印日期到年后2位
                SetBookmarkValue(AgricultureBookMark.BookWriteOneYear + (i == 0 ? "" : i.ToString()), oneYear);
                //打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMark.BookWriteLastYear + (i == 0 ? "" : i.ToString()), twoYear);
                //打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMark.BookWriteMonth + (i == 0 ? "" : i.ToString()), awareMonth);
                //打印日期到月
                SetBookmarkValue(AgricultureBookMark.BookWriteDay + (i == 0 ? "" : i.ToString()), awareday); //打印日期到日
                SetBookmarkValue(AgricultureBookMark.BookAllWriteDate + (i == 0 ? "" : i.ToString()), allAwareString);
                //打印所有颁证日期
            }
        }

        /// <summary>
        /// 填写日期扩展信息
        /// </summary>
        protected override void WriteDateTimeInformation()
        {
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.NowYear + (i == 0 ? "" : i.ToString()),
                    DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMark.YearName + (i == 0 ? "" : i.ToString()),
                    (DateValue != null && DateValue.HasValue) ? DateValue.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckYearName + (i == 0 ? "" : i.ToString()),
                    (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.NowMonth + (i == 0 ? "" : i.ToString()),
                    DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMark.MonthName + (i == 0 ? "" : i.ToString()),
                    (DateValue != null && DateValue.HasValue) ? DateValue.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckMonthName + (i == 0 ? "" : i.ToString()),
                    (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.NowDay + (i == 0 ? "" : i.ToString()), DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMark.DayName + (i == 0 ? "" : i.ToString()),
                    (DateValue != null && DateValue.HasValue) ? DateValue.Value.Day.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckDayName + (i == 0 ? "" : i.ToString()),
                    (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Day.ToString() : "");
                //SetBookmarkValue(AgricultureBookMark.FullDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(DateTime.Now));
                SetBookmarkValue(AgricultureBookMark.FullDate + (i == 0 ? "" : i.ToString()),
                    ToolDateTime.GetLongDateString((DateTime)DateValue));
                string year = ToolMath.GetChineseLowNumber(DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseYearName + (i == 0 ? "" : i.ToString()), year);
                string month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseMonthName + (i == 0 ? "" : i.ToString()), month);
                string day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseDayName + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue(AgricultureBookMark.FullChineseDate + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        /// <summary>
        /// 书写宗地示意图信息
        /// </summary>
        protected override void WriteParcelInformation()
        {
            string drawPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSetting.DRAWPERSON, "");
            string checkPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSetting.VERIFYPERSON, "");
            if (!string.IsNullOrEmpty(drawPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawPerson, drawPerson); //制图人
            }
            DateTime date = DateTime.Now;
            string value = ToolConfiguration.GetSpecialAppSettingValue("GraphDateTime", "");
            try
            {
                value = !string.IsNullOrEmpty(value) ? value.Replace(",", "") : value;
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime.TryParse(value, out date);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (date != null)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawDate, ToolDateTime.GetLongDateString(date));
                //制图日期
            }
            if (!string.IsNullOrEmpty(checkPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckPerson, checkPerson); //审核人
            }
            value = ToolConfiguration.GetSpecialAppSettingValue("CheckDateTime", "");
            try
            {
                value = !string.IsNullOrEmpty(value) ? value.Replace(",", "") : value;
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime.TryParse(value, out date);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (date != null)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckDate, ToolDateTime.GetLongDateString(date));
                //审核日期
            }
        }

        /// <summary>
        /// 书写其他信息
        /// </summary>
        protected override void WriteOtherInformation()
        {
            if (!string.IsNullOrEmpty(ParcelDrawPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawPerson, ParcelDrawPerson); //制图人
            }
            if (ParcelDrawDate != null && ParcelDrawDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawDate,
                    ToolDateTime.GetLongDateString(ParcelDrawDate.Value)); //制图日期
            }
            if (!string.IsNullOrEmpty(ParcelCheckPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckPerson, ParcelCheckPerson); //审核人
            }
            if (ParcelCheckDate != null && ParcelCheckDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckDate,
                    ToolDateTime.GetLongDateString(ParcelCheckDate.Value)); //审核日期
            }
            if (!string.IsNullOrEmpty(DrawTablePerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureDrawTablePerson, DrawTablePerson); //制表人
            }
            if (DrawTableDate != null && DrawTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureDrawTableDate,
                    ToolDateTime.GetLongDateString(DrawTableDate.Value)); //制表日期
            }
            if (!string.IsNullOrEmpty(CheckTablePerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckTablePerson, CheckTablePerson); //检查人
            }
            if (CheckTableDate != null && CheckTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckTableDate,
                    ToolDateTime.GetLongDateString(CheckTableDate.Value)); //制表日期
            }
        }

        /// <summary>
        /// 设置承包类型
        /// </summary>
        protected override void WriteConcordModeInformation()
        {
            if (Concord == null && DictList != null)
            {
                return;
            }
            int number = 0;
            int.TryParse(Concord.ArableLandType, out number);
            if (number <= 0)
                return;
            eConstructMode mode = (eConstructMode)number;
            switch (mode)
            {
                case eConstructMode.Consensus:
                    SetBookmarkValue(AgricultureBookMark.ConsensusContract, "R"); //公开协商
                    break;
                case eConstructMode.Exchange:
                    SetBookmarkValue(AgricultureBookMark.ExchangeContract, "R"); //互换
                    break;
                case eConstructMode.Family:
                    SetBookmarkValue(AgricultureBookMark.FamilyContract, "R"); //家庭承包
                    break;
                case eConstructMode.Other:
                    SetBookmarkValue(AgricultureBookMark.OtherContract, "R"); //其他
                    break;
                case eConstructMode.Tenderee:
                    SetBookmarkValue(AgricultureBookMark.TendereeContract, "R"); //招标
                    break;
                case eConstructMode.Transfer:
                    SetBookmarkValue(AgricultureBookMark.TransferContract, "R"); //转让
                    break;
                case eConstructMode.Vendue:
                    SetBookmarkValue(AgricultureBookMark.VendueContract, "R"); //拍卖
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        protected override void WriteConcordStartAndEndTime()
        {
            if (Concord == null)
            {
                return;
            }
            string startYear = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue)
                ? ""
                : Concord.ArableLandStartTime.Value.Year.ToString();
            string startMonth = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue)
                ? ""
                : Concord.ArableLandStartTime.Value.Month.ToString();
            string startDay = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue)
                ? ""
                : Concord.ArableLandStartTime.Value.Day.ToString();
            string endYear = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue)
                ? ""
                : Concord.ArableLandEndTime.Value.Year.ToString();
            string endMonth = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue)
                ? ""
                : Concord.ArableLandEndTime.Value.Month.ToString();
            string endDay = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue)
                ? ""
                : Concord.ArableLandEndTime.Value.Day.ToString();
            string date = "";
            if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue &&
                Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
            {
                date = "自" +
                       string.Format("{0}年{1}月{2}日", Concord.ArableLandStartTime.Value.Year,
                           Concord.ArableLandStartTime.Value.Month, Concord.ArableLandStartTime.Value.Day) + "起至"
                       +
                       string.Format("{0}年{1}月{2}日", Concord.ArableLandEndTime.Value.Year,
                           Concord.ArableLandEndTime.Value.Month, Concord.ArableLandEndTime.Value.Day) + "止";
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordStartDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Concord.ArableLandStartTime.Value)); //起始时间
                }
                SetBookmarkValue(AgricultureBookMark.ConcordStartYearDate + (i == 0 ? "" : i.ToString()), startYear);
                //起始时间-年
                SetBookmarkValue(AgricultureBookMark.ConcordStartMonthDate + (i == 0 ? "" : i.ToString()), startMonth);
                //起始时间-月
                SetBookmarkValue(AgricultureBookMark.ConcordStartDayDate + (i == 0 ? "" : i.ToString()), startDay);
                //起始时间-日
                if (Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordEndDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Concord.ArableLandEndTime.Value)); //结束时间
                }
                SetBookmarkValue(AgricultureBookMark.ConcordEndYearDate + (i == 0 ? "" : i.ToString()), endYear);
                //起始时间-年
                SetBookmarkValue(AgricultureBookMark.ConcordEndMonthDate + (i == 0 ? "" : i.ToString()), endMonth);
                //起始时间-月
                SetBookmarkValue(AgricultureBookMark.ConcordEndDayDate + (i == 0 ? "" : i.ToString()), endDay); //起始时间-日
                SetBookmarkValue(AgricultureBookMark.ConcordDate + (i == 0 ? "" : i.ToString()), date); //承包时间
                SetBookmarkValue(AgricultureBookMark.ConcordTrem + (i == 0 ? "" : i.ToString()),
                    Concord.Flag ? "长久" : Concord.ManagementTime); //合同期限
                SetBookmarkValue(AgricultureBookMark.ConcordLongTime + (i == 0 ? "" : i.ToString()), "长久"); //合同中长久日期
            }
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        protected override void WriteSharePersonInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            List<Person> persons = SortSharePerson(Contractor.SharePersonList, Contractor.Name);
            string nameList = "";
            foreach (var item in persons)
            {
                nameList += (item.Name == Contractor.Name) ? InitalizeFamilyName(item.Name) : item.Name;
                nameList += "、";
            }
            string cutNameList = string.IsNullOrEmpty(nameList) ? "" : nameList.Substring(0, nameList.Length - 1);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorCount + (i == 0 ? "" : i.ToString()),
                    persons.Count.ToString()); //承包方家庭成员个数
                SetBookmarkValue(AgricultureBookMark.SharePersonCount + (i == 0 ? "" : i.ToString()),
                    persons.Count.ToString()); //承包方家庭成员个数 
                SetBookmarkValue(AgricultureBookMark.SharePersonString + (i == 0 ? "" : i.ToString()), cutNameList);
                //共有人字符串
            }
            Person familyPerson = null;
            int index = 1;
            foreach (Person person in persons)
            {
                if (person.Name == Contractor.Name && person.ICN == Contractor.Number)
                {
                    familyPerson = person.Clone() as Person;
                }
                string name = AgricultureBookMark.SharePersonName + index.ToString();
                SetBookmarkValue(name, person.Name == Contractor.Name ? InitalizeFamilyName(person.Name) : person.Name);
                //共有人姓名
                string gender = AgricultureBookMark.SharePersonGender + index.ToString();
                string sex = person.Gender == eGender.Female ? "女" : (person.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(gender, sex); //共有人性别
                string ageString = AgricultureBookMark.SharePersonAge + index.ToString();
                SetBookmarkValue(ageString, GetAge(person)); //共有人年龄
                string birthString = AgricultureBookMark.SharePersonBirthday + index.ToString();
                SetBookmarkValue(birthString, GetPersonBirthday(person, 0)); //共有人出生日期
                string birthDayString = AgricultureBookMark.SharePersonBirthMonthDay + index.ToString();
                SetBookmarkValue(birthDayString, GetPersonBirthday(person, 2)); //共有人出生日期
                string birthAllString = AgricultureBookMark.SharePersonAllBirthday + index.ToString();
                SetBookmarkValue(birthAllString, GetPersonBirthday(person, 1)); //共有人全出生日期
                string relationString = AgricultureBookMark.SharePersonRelation + index.ToString();
                SetBookmarkValue(relationString, person.Relationship); //家庭关系
                string icnNumber = AgricultureBookMark.SharePersonNumber + index.ToString();
                SetBookmarkValue(icnNumber, person.ICN); //身份证号码
                string nation = AgricultureBookMark.SharePersonNation + index.ToString();
                string nationString = EnumNameAttribute.GetDescription(person.Nation);
                SetBookmarkValue(nation, nationString == "未知" ? "" : nationString); //共有人民族
                string nature = AgricultureBookMark.SharePersonAccountNature + index.ToString();
                SetBookmarkValue(nature, person.AccountNature); //共有人性质
                string comment = AgricultureBookMark.SharePersonComment + index.ToString();
                SetBookmarkValue(comment, person.Comment); //备注
                index++;
            }
            persons = null;
            if (familyPerson == null)
            {
                return;
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                string sex = familyPerson.Gender == eGender.Female
                    ? "女"
                    : (familyPerson.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(AgricultureBookMark.ContractorGender + (i == 0 ? "" : i.ToString()), sex); //承包方性别
                SetBookmarkValue(AgricultureBookMark.ContractorAge + (i == 0 ? "" : i.ToString()), GetAge(familyPerson));
                //承包方年龄
                SetBookmarkValue(AgricultureBookMark.ContractorBirthday + (i == 0 ? "" : i.ToString()),
                    GetPersonBirthday(familyPerson, 0)); //承包方初始日期
                SetBookmarkValue(AgricultureBookMark.ContractorAllBirthday + (i == 0 ? "" : i.ToString()),
                    GetPersonBirthday(familyPerson, 1)); //承包方初始日期
                SetBookmarkValue(AgricultureBookMark.ContractorBirthMonthDay + (i == 0 ? "" : i.ToString()),
                    GetPersonBirthday(familyPerson, 2)); //承包方初始日期
            }
        }

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        protected override void WriteReclamationInformation()
        {
            List<ContractLand> landCollection = LandCollection.Clone() as List<ContractLand>;
            List<ContractLand> landArray =
                landCollection.FindAll(ld => (!string.IsNullOrEmpty(ld.Comment) && ld.Comment.IndexOf("开垦地") >= 0));
            double reclamationTableArea = 0.0; //开垦地台帐面积
            double reclamationActualArea = 0.0; //开垦地实测面积
            double reclamationAwareArea = 0.0; //开垦地确权面积
            foreach (ContractLand land in landArray)
            {
                reclamationTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                reclamationActualArea += land.ActualArea;
                reclamationAwareArea += land.AwareArea;
                landCollection.Remove(land);
            }
            double retainTableArea = 0.0;
            double retainActualArea = 0.0;
            double retainAwareArea = 0.0;
            foreach (ContractLand land in landCollection)
            {
                retainTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                retainActualArea += land.ActualArea;
                retainAwareArea += land.AwareArea;
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationTableArea + (i == 0 ? "" : i.ToString()),
                    reclamationTableArea.AreaFormat()); //台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainTableArea + (i == 0 ? "" : i.ToString()),
                    retainTableArea.AreaFormat()); //台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationActualArea + (i == 0 ? "" : i.ToString()),
                    reclamationActualArea.AreaFormat()); //实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainActualArea + (i == 0 ? "" : i.ToString()),
                    retainActualArea.AreaFormat()); //实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationAwareArea + (i == 0 ? "" : i.ToString()),
                    reclamationAwareArea.AreaFormat()); //确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainAwareArea + (i == 0 ? "" : i.ToString()),
                    retainAwareArea.AreaFormat()); //确权面积
            }
            landCollection.Clear();
            landArray.Clear();
        }

        /// <summary>
        /// 书写地块计算信息
        /// </summary>
        protected override void WriteLandCalInformation()
        {
            List<ContractLand> lands = LandCollection.FindAll(ld => ld.LandCategory == ConstructType);
            List<ContractLand> otherLands = LandCollection.FindAll(ld => ld.LandCategory != ConstructType);
            double actualArea = 0.0;
            double conLandActualArea = 0.0;
            double othLandActualArea = 0.0;
            double awareArea = 0.0;
            double conLandawareArea = 0.0;
            double othLandawareArea = 0.0;
            double tableArea = 0.0;
            double conLandtableArea = 0.0;
            double othLandtableArea = 0.0;
            double modoArea = 0.0;
            double conLandmodoArea = 0.0;
            double othLandmodoArea = 0.0;
            foreach (ContractLand land in LandCollection)
            {
                actualArea += land.ActualArea;
                awareArea += land.AwareArea;
                tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                modoArea += (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue)
                    ? land.MotorizeLandArea.Value
                    : 0.0;
            }
            foreach (ContractLand land in lands)
            {
                conLandActualArea += land.ActualArea;
                conLandawareArea += land.AwareArea;
                conLandtableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                conLandmodoArea += (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue)
                    ? land.MotorizeLandArea.Value
                    : 0.0;
            }
            othLandActualArea = actualArea - conLandActualArea;
            othLandawareArea = awareArea - conLandawareArea;
            othLandtableArea = tableArea - conLandtableArea;
            othLandmodoArea = modoArea - conLandmodoArea;
            for (int j = 0; j < BookMarkCount; j++)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCount + (j == 0 ? "" : j.ToString()),
                    LandCollection.Count < 1 ? "  " : LandCollection.Count.ToString()); //地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandCount + (j == 0 ? "" : j.ToString()),
                    (lands != null && lands.Count > 0) ? lands.Count.ToString() : ""); //承包地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherCount + (j == 0 ? "" : j.ToString()), otherLands.Count.ToString()); //非承包地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureActualAreaCount + (j == 0 ? "" : j.ToString()), actualArea.AreaFormat()); //地块总实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureAwareAreaCount + (j == 0 ? "" : j.ToString()),
                    awareArea.AreaFormat()); //地块总确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureTableAreaCount + (j == 0 ? "" : j.ToString()),
                    tableArea.AreaFormat()); //地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.AgricultureModoAreaCount + (j == 0 ? "" : j.ToString()),
                    modoArea.AreaFormat()); //地块总机动地面积
                SetBookmarkValue(
                    AgricultureBookMark.AgricultureContractLandActualAreaCount + (j == 0 ? "" : j.ToString()),
                    conLandActualArea.AreaFormat()); //地块总实测面积
                SetBookmarkValue(
                    AgricultureBookMark.AgricultureContractLandAwareAreaCount + (j == 0 ? "" : j.ToString()),
                    conLandawareArea.AreaFormat()); //地块总确权面积
                SetBookmarkValue(
                    AgricultureBookMark.AgricultureContractLandTableAreaCount + (j == 0 ? "" : j.ToString()),
                    conLandtableArea.AreaFormat()); //地块总二轮台账面积
                SetBookmarkValue(
                    AgricultureBookMark.AgricultureContractLandModoAreaCount + (j == 0 ? "" : j.ToString()),
                    conLandmodoArea.AreaFormat()); //地块总机动地面积
                SetBookmarkValue(
                    AgricultureBookMark.AgricultureOtherLandActualAreaCount + (j == 0 ? "" : j.ToString()),
                    othLandActualArea.AreaFormat()); //地块总实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandAwareAreaCount + (j == 0 ? "" : j.ToString()),
                    othLandawareArea.AreaFormat()); //地块总确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandTableAreaCount + (j == 0 ? "" : j.ToString()),
                    othLandtableArea.AreaFormat()); //地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandModoAreaCount + (j == 0 ? "" : j.ToString()),
                    othLandmodoArea.AreaFormat()); //地块总机动地面积
            }
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        protected override List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                if (landNumber.Length > 14)
                {
                    landNumber = landNumber.Substring(14);
                }
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        /// <summary>
        /// 初始化四至
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        protected override string InitalizeLandNeightor(ContractLand land)
        {
            string neighbor = string.Format("东：{0}\n南：{1}\n西：{2} \n北：{3}", land.NeighborEast, land.NeighborSouth,
                land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("东：{0}\n西：{1}\n南：{2} \n北：{3}", land.NeighborEast, land.NeighborWest,
                    land.NeighborSouth, land.NeighborNorth);
            }
            return neighbor;
        }

        #endregion



        /// <summary>
        /// 获取等级
        /// </summary>
        protected new string InitalizeLandLevel(string landLevel)
        {
            if (DictList == null)
            {
                return "";
            }
            Dictionary dic = DictList.Find(t => t.Code == landLevel);
            if (dic == null)
            {
                return "";
            }
            return dic.Name;
        }

        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public new string InitalizeFamilyName(string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "";
            }
            string number = ToolString.GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }

        /// <summary>
        /// 获得承包方编码
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        protected string InitalizeContractorCode(VirtualPerson contractor, Zone zone)
        {
            if (contractor == null || zone == null)
            {
                return "";
            }
            int number = 0;
            Int32.TryParse(contractor.FamilyNumber, out number);
            string familyNumber = null;
            switch (zone.Level)
            {
                case eZoneLevel.Village:
                    familyNumber = zone.FullCode + "00" + string.Format("{0:D4}", number);
                    break;
                case eZoneLevel.Group:
                    //familyNumber = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14, 2) + string.Format("{0:D4}", number);
                    familyNumber = zone.FullCode + string.Format("{0:D4}", number);
                    break;
                default:
                    break;
            }
            return familyNumber;
        }

        /// <summary>
        /// 初始化面积
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        protected double InitalizeArea(double area, int dicimal = 4)
        {
            double ae = Math.Round(area, dicimal);
            return ae;
        }

        /// <summary>
        /// 复制表  
        /// </summary>
        /// <param name="startRowIndex">复制起始行序号</param>
        /// <param name="pageRowNum">复制一页的空白行数</param>
        /// <param name="fisrtPageRemainRow">第一页剩余行数</param>
        /// <param name="tableIndex">表，默认第一个表</param>
        protected void CopyTable(int startRowIndex, int pageRowNum, int fisrtPageRemainRow, int tableIndex = 0)
        {
            int pageNum = 0;
            if (LandCollection.Count - fisrtPageRemainRow > 0)
                pageNum = (LandCollection.Count - fisrtPageRemainRow) / pageRowNum + 1; //计算出要复制几页
            for (int i = 0; i < pageNum; i++)
            {
                InsertTableRowClone(tableIndex, startRowIndex);
                InsertTableRowClone(tableIndex, startRowIndex + 1, pageRowNum);
                _titleInRowList.Add(startRowIndex + fisrtPageRemainRow + i * (pageRowNum + 1) + 1);
                //记录标题所在行序号 往表格写入数据时跳过此行
                LandCollection.Insert(8 + i * 14, new ContractLand());
                //在标题所在行添加一条空数据，以便填数据时跳过此行
            }
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary> 
        protected new string GetZoneNameByLevel(string zoneCode, YuLinTu.Library.Entity.eZoneLevel level, Library.WorkStation.IZoneWorkStation zoneStation)
        {
            YuLinTu.Library.Entity.Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            return GetZoneNameByLevel(temp.UpLevelCode, level, zoneStation);
        }

    }


    public static class StringExtend
    {
        /// <summary>
        /// 截取字符串后几位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        //public static string GetLastString(this string value, int number)
        //{
        //    if (string.IsNullOrEmpty((value)) || value.Count() <= number)
        //    {
        //        return value;
        //    }
        //    return value.Substring(value.Length - number, number);
        //}



        /// <summary>
        /// 鑾峰彇瀛楃涓诧紝濡傛灉涓簄ull 绌?鍒欒繑鍥炩€?鈥?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetExportString(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
        }
    }
}
