/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 调查信息汇总表类
    /// </summary>
    public class ExportSurveySummarizeTable : AgricultureWordBook
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportSurveySummarizeTable()
        {
        }

        #endregion

        #region Fields

        private int tableIndex;     //表格索引
        private List<YuLinTu.Library.Entity.VirtualPerson> familyModeVps;  //家庭承包方式承包方集合
        private List<YuLinTu.Library.Entity.VirtualPerson> otherModeVps;  // 其他承包方式承包方集合
        private List<ContractLand> familyModeLands;  //家庭承包方式承包地块集合
        private List<ContractLand> otherModeLands;  //其他承包方式承包地块集合
        private List<ContractConcord> familyModeConcords;  //家庭承包方式承包合同集合
        private List<ContractConcord> otherModeConcords;  //其他承包方式承包合同集合
        private List<YuLinTu.Library.Entity.VirtualPerson> unitVps;  //承包方为单位
        private List<YuLinTu.Library.Entity.VirtualPerson> personalVps;  //承包方为个人
        private List<ContractLand> villageCollectiveLands;  //村集体
        private List<ContractLand> groupOfPeopleLands; //村民组集体
        private List<ContractLand> familyModeotherLands; //国有

        private const double format = 0.0666667;  //亩到公顷转化系数

        #endregion

        #region Properties

        /// <summary>
        /// 指定行政地域下的承包方集合
        /// </summary>
        public List<YuLinTu.Library.Entity.VirtualPerson> ListVp { get; set; }

        /// <summary>
        /// 指定行政地域下的地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 指定行政地域下的合同集合
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 指定行政地域下的权证集合
        /// </summary>
        public List<YuLinTu.Library.Entity.ContractRegeditBook> ListContractRegeditBook { get; set; }

        /// <summary>
        /// 县级地域名称
        /// </summary>
        public string ZoneNameCounty { get; set; }

        /// <summary>
        /// 镇级地域名称
        /// </summary>
        public string ZoneNameTown { get; set; }

        /// <summary>
        /// 村级地域名称
        /// </summary>
        public string ZoneNameVillage { get; set; }

        /// <summary>
        /// 组级地域名称
        /// </summary>
        public string ZoneNameGroup { get; set; }

        #endregion

        #region Methods

        #region Methods - Override

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
                return false;
            try
            {
                if (!CheckDataInformation(data))
                {
                    return false;
                }
                InitalizeDataInformation(data);

                WriteTitleInformation();
                WriteFamilyModeContractor();
                WriteOtherModeContractor();

                WriteFamilyModeLandInformation();
                WriteOtherModeLandInformation();

                WriteFamilyModeConcordInformation();
                WriteOtherModeConcordInformation();


                Disponse();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSetParamValue(导出承包合同)", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeDataInformation(object data)
        {
            tableIndex = 0;
            CurrentZone = CurrentZone.Clone() as Zone;
            LandCollection = ListLand.Clone() as List<ContractLand>;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            //base.Destroyed();
            //CurrentZone = null;
            //ListLand = null;
            //familyModeVps = null;
            //otherModeVps = null;
            //familyModeLands = null;
            //otherModeLands = null;
            //familyModeConcords = null;
            //otherModeConcords = null;
            //unitVps = null;
            //personalVps = null;
            //villageCollectiveLands = null;
            //groupOfPeopleLands = null;
            //statedLands = null;
            GC.Collect();
        }

        #endregion

        #region Methods - Family

        /// <summary>
        /// 填写家庭承包经营的承包方信息
        /// </summary>
        private void WriteFamilyModeContractor()
        {
            familyModeVps = ListVp.FindAll(c => c.FamilyExpand.ConstructMode == eConstructMode.Family);  //家庭承包
            if (familyModeVps == null)
                return;
            SetTableCellValue(tableIndex, 1, 3, familyModeVps.Count == 0 ? "" : familyModeVps.Count.ToString());
        }

        /// <summary>
        /// 填写其他承包方式经营的承包方信息
        /// </summary>
        private void WriteOtherModeContractor()
        {
            otherModeVps = ListVp.FindAll(c => c.FamilyExpand.ConstructMode != eConstructMode.Family);  //其他承包方式
            if (otherModeVps == null)
                return;
            SetTableCellValue(tableIndex, 10, 3, otherModeVps.Count == 0 ? "" : otherModeVps.Count.ToString());
            unitVps = otherModeVps.FindAll(c => c.FamilyExpand.ContractorType == eContractorType.Unit);  //单位
            personalVps = otherModeVps.FindAll(t => t.FamilyExpand.ContractorType == eContractorType.Personal);  //个人
            if (unitVps != null)
                SetTableCellValue(tableIndex, 11, 3, unitVps.Count == 0 ? "" : unitVps.Count.ToString());
            if (personalVps != null)
                SetTableCellValue(tableIndex, 12, 3, personalVps.Count == 0 ? "" : personalVps.Count.ToString());

            int allpersonscount = 0;
            ListVp.ForEach(vp => { allpersonscount += vp.SharePersonList.Count; });
            SetTableCellValue(tableIndex, 26, 3, allpersonscount == 0 ? "" : allpersonscount.ToString());

        }

        #endregion

        #region Methods - ContractLand

        /// <summary>
        /// 填写家庭承包经营的地块信息
        /// </summary>
        private void WriteFamilyModeLandInformation()
        {
            familyModeLands = ListLand.FindAll(c => c.ConstructMode == ((int)eConstructMode.Family).ToString()); //家庭承包
            if (familyModeLands == null)
                return;
            villageCollectiveLands = familyModeLands.FindAll(c => c.OwnRightType == ((int)eLandPropertyType.VillageCollective).ToString()); //村集体
            groupOfPeopleLands = familyModeLands.FindAll(c => c.OwnRightType == ((int)eLandPropertyType.GroupOfPeople).ToString());  //村民组集体
            //statedLands = familyModeLands.FindAll(c => c.OwnRightType == ((int)eLandPropertyType.Stated).ToString());  //国有   
            //if (villageCollectiveLands == null)
            //    villageCollectiveLands = new List<ContractLand>();
            //if (groupOfPeopleLands == null)
            //    groupOfPeopleLands = new List<ContractLand>();
            //if (statedLands == null)
            //    statedLands = new List<ContractLand>();

            //familyModeotherLands = familyModeLands.FindAll(c => c.OwnRightType == ((int)eLandPropertyType.VillageCollective).ToString()); //村集体

            double Actualareas = 0.0;
            foreach (var land in familyModeLands)
            {
                Actualareas += land.ActualArea;
            }
            double villageCollectivelandActualareas = 0.0;
            foreach (var land in villageCollectiveLands)
            {
                villageCollectivelandActualareas += land.ActualArea;
            }
            double groupOfPeopleLandActualareas = 0.0;
            foreach (var land in groupOfPeopleLands)
            {
                groupOfPeopleLandActualareas += land.ActualArea;
            }
            var otherActualarea = Actualareas - villageCollectivelandActualareas - groupOfPeopleLandActualareas;

            SetTableCellValue(tableIndex, 6, 3, (Math.Round(Actualareas * format, 2) == 0 ? "" : Math.Round(Actualareas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 7, 3, (Math.Round(villageCollectivelandActualareas * format, 2) == 0 ? "" : Math.Round(villageCollectivelandActualareas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 8, 3, (Math.Round(groupOfPeopleLandActualareas * format, 2) == 0 ? "" : Math.Round(groupOfPeopleLandActualareas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 9, 3, (Math.Round(otherActualarea * format, 2) == 0 ? "" : Math.Round(otherActualarea * format, 2).ToString()));


            //areas = 0.0;
            //foreach (var land in villageCollectiveLands)
            //{
            //    areas += land.ActualArea;
            //}
            //SetTableCellValue(tableIndex, 7, 3, Math.Round(areas * format, 4).ToString());
            //areas = 0.0;
            //foreach (var land in groupOfPeopleLands)
            //{
            //    areas += land.ActualArea;
            //}
            //SetTableCellValue(tableIndex, 8, 3, Math.Round(areas * format, 4).ToString());
            //areas = 0.0;
            //foreach (var land in statedLands)
            //{
            //    areas += land.ActualArea;
            //}
            //SetTableCellValue(tableIndex, 9, 3, Math.Round(areas * format, 4).ToString());

            //SetTableCellValue(tableIndex, 8, 3, (Math.Round(areas * format, 2) == 0 ? "" : Math.Round(areas * format, 2).ToString()));
        }

        /// <summary>
        /// 填写其他承包方式经营的地块信息
        /// </summary>
        private void WriteOtherModeLandInformation()
        {
            otherModeLands = ListLand.FindAll(c => c.ConstructMode != ((int)eConstructMode.Family).ToString());   //其他承包方式
            if (otherModeLands == null)
                return;
            double allAreas = 0.0;
            double unitLandAreas = 0.0;     //单位承包
            double personalLandAreas = 0.0;  //个人承包
            //if (unitVps == null)
            //    unitVps = new List<YuLinTu.Library.Entity.VirtualPerson>();
            //if (personalVps == null)
            //    personalVps = new List<YuLinTu.Library.Entity.VirtualPerson>();
            //foreach (var land in otherModeLands)
            //{
            //    allAreas += land.ActualArea;
            //    if (unitVps.Any(c => c.ID == land.OwnerId))
            //        unitLandAreas += land.ActualArea;
            //    else if (personalVps.Any(t => t.ID == land.OwnerId))
            //        personalLandAreas += land.ActualArea;
            //}

            foreach (var land in otherModeLands)
            {
                allAreas += land.ActualArea;
                if (ListVp.Any(c => c.ID == land.OwnerId && c.FamilyExpand.ContractorType == eContractorType.Unit))
                    unitLandAreas += land.ActualArea;
                else if (ListVp.Any(c => c.ID == land.OwnerId && c.FamilyExpand.ContractorType == eContractorType.Personal))
                    personalLandAreas += land.ActualArea;
            }
            SetTableCellValue(tableIndex, 16, 3, (Math.Round(allAreas * format, 2) == 0 ? "" : Math.Round(allAreas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 17, 3, (Math.Round(unitLandAreas * format, 2) == 0 ? "" : Math.Round(unitLandAreas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 18, 3, (Math.Round(personalLandAreas * format, 2) == 0 ? "" : Math.Round(personalLandAreas * format, 2).ToString()));
        }
        #endregion

        #region Methods - Concord

        /// <summary>
        /// 填写家庭承包经营的合同、权证信息
        /// </summary>
        private void WriteFamilyModeConcordInformation()
        {
            familyModeConcords = ListConcord.FindAll(c => c.ArableLandType == ((int)eConstructMode.Family).ToString()); //家庭承包
            if (familyModeConcords == null)
                return;
            SetTableCellValue(tableIndex, 19, 3, (ListConcord.Count == 0 ? "" : ListConcord.Count.ToString()));
            SetTableCellValue(tableIndex, 20, 3, (familyModeConcords.Count == 0 ? "" : familyModeConcords.Count.ToString()));
            SetTableCellValue(tableIndex, 22, 3, (ListContractRegeditBook.Count == 0 ? "" : ListContractRegeditBook.Count.ToString()));
            List<YuLinTu.Library.Entity.ContractRegeditBook> familyModeWarrantlist = new List<YuLinTu.Library.Entity.ContractRegeditBook>();

            double allAreas = 0.0;
            double villageCollectiveConcordAreas = 0.0;     //村集体
            double groupOfPeopleConcordAreas = 0.0;  //村民组集体
            double otherConcordAreas = 0.0;  //其它
            foreach (var concord in familyModeConcords)
            {
                var familyModeWarrant = ListContractRegeditBook.Find(r => r.ID == concord.ID);
                if (familyModeWarrant != null)
                    familyModeWarrantlist.Add(familyModeWarrant);
                allAreas += concord.CountActualArea;
                if (villageCollectiveLands.Any(c => c.ConcordId == concord.ID))
                    villageCollectiveConcordAreas += concord.CountActualArea;
                else if (groupOfPeopleLands.Any(c => c.ConcordId == concord.ID))
                    groupOfPeopleConcordAreas += concord.CountActualArea;

            }
            SetTableCellValue(tableIndex, 23, 3, (familyModeWarrantlist.Count == 0 || familyModeWarrantlist == null ? "" : familyModeWarrantlist.Count.ToString()));
         
            otherConcordAreas = allAreas - villageCollectiveConcordAreas - groupOfPeopleConcordAreas;
            SetTableCellValue(tableIndex, 2, 3, (Math.Round(allAreas * format, 2) == 0 ? "" : Math.Round(allAreas * format, 2).ToString()));
            SetTableCellValue(tableIndex, 3, 3, Math.Round(villageCollectiveConcordAreas * format, 2).ToString());
            SetTableCellValue(tableIndex, 4, 3, Math.Round(groupOfPeopleConcordAreas * format, 2).ToString());
            SetTableCellValue(tableIndex, 5, 3, Math.Round(otherConcordAreas * format, 2).ToString());
            
        }

        /// <summary>
        /// 填写其他承包方式经营的合同信息
        /// </summary>
        private void WriteOtherModeConcordInformation()
        {
            otherModeConcords = ListConcord.FindAll(c => c.ArableLandType != ((int)eConstructMode.Family).ToString());  //其他承包方式
            if (otherModeConcords == null)
                return;
            double allAreas = 0.0;
            List<YuLinTu.Library.Entity.ContractRegeditBook> otherModeWarrantlist = new List<YuLinTu.Library.Entity.ContractRegeditBook>();
            SetTableCellValue(tableIndex, 21, 3, (otherModeConcords.Count == 0 ? "" : otherModeConcords.Count.ToString()));
            otherModeConcords.ForEach(c =>
            {
                allAreas += c.CountActualArea;
                var otherModeWarrant = ListContractRegeditBook.Find(r => r.ID == c.ID);
                if (otherModeWarrant != null)
                    otherModeWarrantlist.Add(otherModeWarrant);
            });
            SetTableCellValue(tableIndex, 24, 3, (otherModeWarrantlist.Count == 0 || otherModeWarrantlist == null ? "" : otherModeWarrantlist.Count.ToString()));

            SetTableCellValue(tableIndex, 13, 3, (Math.Round(allAreas * format, 2) == 0 ? "" : Math.Round(allAreas * format, 2).ToString()));
            var unitConcord = otherModeConcords.FindAll(c => c.ContracerType == ((int)eContractorType.Unit).ToString());  //单位
            var personalConcord = otherModeConcords.FindAll(t => t.ContracerType == ((int)eContractorType.Personal).ToString()); //个人
            if (unitConcord == null)
                unitConcord = new List<ContractConcord>();
            double unitAreas = 0.0;
            unitConcord.ForEach(c => { unitAreas += c.CountActualArea; });
            SetTableCellValue(tableIndex, 14, 3, (Math.Round(unitAreas * format, 2) == 0 ? "" : Math.Round(unitAreas * format, 2).ToString()));
            if (personalConcord == null)
                personalConcord = new List<ContractConcord>();
            double personalAreas = 0.0;
            personalConcord.ForEach(c => { personalAreas += c.CountActualArea; });
            SetTableCellValue(tableIndex, 15, 3, (Math.Round(personalAreas * format, 2) == 0 ? "" : Math.Round(personalAreas * format, 2).ToString()));
        }

        /// <summary>
        /// 填写其他信息
        /// </summary>
        private void WriteOtherInformation()
        {
            //    int personCount = 0;
            //    foreach (var vp in ListVp)
            //    {
            //        personCount += vp.SharePersonList.Count;
            //    }
            //    SetTableCellValue(tableIndex, 20, 3, personCount == 0 ? "" : personCount.ToString());
            //    SetTableCellValue(tableIndex, 21, 3, ListLand.Count == 0 ? "" : ListLand.Count.ToString());
        }

        #endregion

        #region Methods - OtherInfomation

        /// <summary>
        /// 检查数据
        /// </summary>
        private bool CheckDataInformation(object data)
        {
            //检查地域数据
            if (CurrentZone == null)
            {
                return false;
            }
            //检查承包方数据
            ListVp = data as List<YuLinTu.Library.Entity.VirtualPerson>;
            if (ListVp == null)
            {
                ListVp = new List<YuLinTu.Library.Entity.VirtualPerson>();
            }
            //得到地块集合
            if (ListLand == null)
            {
                ListLand = new List<ContractLand>();
            }
            //地块排序
            ListLand = SortLandCollection(ListLand);
            return true;
        }

        /// <summary>
        /// 写表头信息( 县、乡镇、村、组四级汇总)
        /// </summary>
        private void WriteTitleInformation()
        {
            string zoneName = ZoneNameCounty + ZoneNameTown + ZoneNameVillage + ZoneNameGroup;
            SetBookmarkValue("ZoneName", zoneName);  //地域名称
            //SetBookmarkValue("SmallCounty", ZoneNameCounty.Substring(0, ZoneNameCounty.Length - 1));  //县级地域名称
            //SetBookmarkValue("SmallTown", ZoneNameTown.Substring(0, ZoneNameTown.Length - 1));  //镇级地域名称
            //SetBookmarkValue("SmallVillage", ZoneNameVillage.Substring(0, ZoneNameVillage.Length - 1));  //村级地域名称
            //SetBookmarkValue("SmallGroup", ZoneNameGroup.Substring(0, ZoneNameGroup.Length - 1));  //组级地域名称
        }

        #endregion

        #region Methods - 辅助方法

        /// <summary>
        /// 宗地排序
        /// </summary>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ld.LandNumber;
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
            //lands.Clear();
            return landCollection;
        }

        #endregion

        #endregion
    }
}

