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
    public class ExportZYSurveySummarizeTable : AgricultureWordBook
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportZYSurveySummarizeTable()
        {
        }

        #endregion

        #region Fields

        private int tableIndex;     //表格索引      
        
        private List<ContractLand> otherModeLands;  //其他承包方式承包地块集合
        private List<ContractConcord> familyModeConcords;  //家庭承包方式承包合同集合
        private List<ContractConcord> otherModeConcords;  //其他承包方式承包合同集合       

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

                WriteFamilyModeLandInformation();
                

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
           
            GC.Collect();
        }

        #endregion
               
        #region Methods - ContractLand

        /// <summary>
        /// 填写家庭承包经营的地块信息
        /// </summary>
        private void WriteFamilyModeLandInformation()
        {            

            var gdcode = DictList.Find(c=>c.Name=="耕地").Code;
            var gdlands= ListLand.FindAll(c => c.LandCode.StartsWith(gdcode) || c.LandName == "耕地");  //耕地            
            double gdarea = 0.0;
            double gdgyarea = 0.0;
            double gdjddarea = 0.0;
            if (gdlands != null && gdlands.Count > 0)
            {              
                foreach (var item in gdlands)
                {
                    gdarea += item.ActualArea;
                    if (item.OwnRightType == ((int)eLandPropertyType.Stated).ToString())
                    {
                        gdgyarea += item.ActualArea;
                    }
                    if (item.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString())
                    {
                        gdjddarea += item.ActualArea;
                    }
                }
            }

            SetTableCellValue(tableIndex, 2, 1, (Math.Round(gdarea, 2) == 0 ? "" : Math.Round(gdarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 2, (Math.Round(gdgyarea, 2) == 0 ? "" : Math.Round(gdgyarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 3, (Math.Round(gdjddarea, 2) == 0 ? "" : Math.Round(gdjddarea, 2).ToString()));

            var ldcode = DictList.Find(c => c.Name == "林地").Code;
            var ldlands = ListLand.FindAll(c => c.LandCode.StartsWith(ldcode) || c.LandName == "林地");  //林地   
            double ldarea = 0.0;
            double ldgyarea = 0.0;
            if (ldlands != null && ldlands.Count > 0)
            {             
                foreach (var item in ldlands)
                {
                    ldarea += item.ActualArea;
                    if (item.OwnRightType == ((int)eLandPropertyType.Stated).ToString())
                    {
                        ldgyarea += item.ActualArea;
                    }
                }
            }
            SetTableCellValue(tableIndex, 2, 4, (Math.Round(ldarea, 2) == 0 ? "" : Math.Round(ldarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 5, (Math.Round(ldgyarea, 2) == 0 ? "" : Math.Round(ldgyarea, 2).ToString()));

            var cdcode = DictList.Find(c => c.Name == "草地").Code;
            var cdlands = ListLand.FindAll(c => c.LandCode.StartsWith(cdcode) || c.LandName == "草地");  //   
            double cdarea = 0.0;
            double cdgyarea = 0.0;
            if (cdlands != null && cdlands.Count > 0)
            {
                foreach (var item in cdlands)
                {
                    cdarea += item.ActualArea;
                    if (item.OwnRightType == ((int)eLandPropertyType.Stated).ToString())
                    {
                        cdgyarea += item.ActualArea;
                    }
                }
            }
            SetTableCellValue(tableIndex, 2, 6, (Math.Round(cdarea, 2) == 0 ? "" : Math.Round(cdarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 7, (Math.Round(cdgyarea, 2) == 0 ? "" : Math.Round(cdgyarea, 2).ToString()));
            //开荒地
            var khdlands = ListLand.FindAll(c=>c.LandCategory == ((int)eLandCategoryType.WasteLand).ToString());  //   
            double khdarea = 0.0;
            double khdgyarea = 0.0;
            if (khdlands != null && khdlands.Count > 0)
            {
                foreach (var item in khdlands)
                {
                    khdarea += item.ActualArea;
                    if (item.OwnRightType == ((int)eLandPropertyType.Stated).ToString())
                    {
                        khdgyarea += item.ActualArea;
                    }
                }
            }
            SetTableCellValue(tableIndex, 2, 8, (Math.Round(khdarea, 2) == 0 ? "" : Math.Round(khdarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 9, (Math.Round(khdgyarea, 2) == 0 ? "" : Math.Round(khdgyarea, 2).ToString()));

            var sksmcode = DictList.Find(c => c.Name == "水域及水利设施用地").Code;
            var sksmlands = ListLand.FindAll(c => c.LandCode.StartsWith(sksmcode));  //水面  

                        
            List<ContractLand> smlands = new List<ContractLand>();
            if (sksmlands != null)
            {
                smlands.AddRange(sksmlands);
            }           
            double smarea = 0.0;
            double smgyarea = 0.0;
            if (smlands.Count > 0)
            {
                foreach (var item in smlands)
                {
                    smarea += item.ActualArea;
                    if (item.OwnRightType == ((int)eLandPropertyType.Stated).ToString())
                    {
                        smgyarea += item.ActualArea;
                    }
                }
            }
            SetTableCellValue(tableIndex, 2, 10, (Math.Round(smarea, 2) == 0 ? "" : Math.Round(smarea, 2).ToString()));
            SetTableCellValue(tableIndex, 2, 11, (Math.Round(smgyarea, 2) == 0 ? "" : Math.Round(smgyarea, 2).ToString()));

            double ALLActualareas = 0.0;
            foreach (var land in ListLand)
            {
                ALLActualareas += land.ActualArea;
            }

            var otherActualarea = ALLActualareas - gdarea - ldarea - cdarea  - smarea;

            SetTableCellValue(tableIndex, 2, 12, (Math.Round(otherActualarea, 2) == 0 ? "" : Math.Round(otherActualarea, 2).ToString()));


        }

      
        #endregion

        #region Methods - Concord
        
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
            SetTableCellValue(tableIndex, 2, 0, zoneName);          
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
           // lands.Clear();
            return landCollection;
        }

        #endregion

        #endregion
    }
}

