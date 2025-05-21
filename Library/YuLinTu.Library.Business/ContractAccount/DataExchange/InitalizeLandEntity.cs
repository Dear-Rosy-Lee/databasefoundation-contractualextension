/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.PropertyRight;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包经营权实体
    /// </summary>
    public class AgriLandEntity
    {
        #region Propertys

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> Contractors { get; set; }

        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> Lands { get; set; }

        /// <summary>
        /// 承包合同集合
        /// </summary>
        public List<ContractConcord> Concords { get; set; }

        /// <summary>
        /// 承包权证
        /// </summary>
        public List<ContractRegeditBook> Books { get; set; }

        /// <summary>
        /// 申请表
        /// </summary>
        public List<ContractRequireTable> Tables { get; set; }

        /// <summary>
        /// 地类
        /// </summary>
        public List<LandType> LandTypes { get; set; }

        /// <summary>
        /// 集体经济组织集合
        /// </summary>
        public List<CollectivityTissue> Tissues { get; set; }

        /// <summary>
        /// 界址点信息
        /// </summary>
        public List<BuildLandBoundaryAddressDot> DotCollection { get; set; }

        /// <summary>
        /// 界址线信息
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> LineCollection { get; set; }

        #endregion

        #region Ctor

        public AgriLandEntity()
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Config\LandType.xml";
            try
            {
                LandTypes = ToolSerialization.DeserializeXml(fileName, typeof(List<LandType>)) as List<LandType>;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            InitalizeDataList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitalizeDataList()
        {
            Tissues = new List<CollectivityTissue>();
            Contractors = new List<VirtualPerson>();
            Lands = new List<ContractLand>();
            Concords = new List<ContractConcord>();
            DotCollection = new List<BuildLandBoundaryAddressDot>();
            LineCollection = new List<BuildLandBoundaryAddressCoil>();
            Books = new List<ContractRegeditBook>();
            Tables = new List<ContractRequireTable>();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitalizeCadaZoneData(IDbContext db, string zoneCode)
        {
            Tissues = new List<CollectivityTissue>();
            Contractors = new List<VirtualPerson>();
            Lands = new List<ContractLand>();
            Concords = new List<ContractConcord>();
            DotCollection = new List<BuildLandBoundaryAddressDot>();
            LineCollection = new List<BuildLandBoundaryAddressCoil>();
            Books = new List<ContractRegeditBook>();
            Tables = new List<ContractRequireTable>();
            if (string.IsNullOrEmpty(zoneCode))
            {
                return;
            }
            if (db != null)
            {
                //CurrentZone = db.Zone.Get(zoneCode);
                Contractors = new List<VirtualPerson>();
                //Lands = db.ContractLand.SL_GetCollection("CadastralZoneCode", zoneCode, Library.Data.ConditionOption.Like_LeftFixed);
                Concords = new List<ContractConcord>();
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitalizeZoneData(IDbContext db, string zoneCode)
        {
            Tissues = new  List<CollectivityTissue>();
            Contractors = new  List<VirtualPerson>();
            Lands = new List<ContractLand>();
            Concords = new List<ContractConcord>();
            DotCollection = new List<BuildLandBoundaryAddressDot>();
            LineCollection = new  List<BuildLandBoundaryAddressCoil>();
            Books = new List<ContractRegeditBook>();
            Tables = new List<ContractRequireTable>();
            if (string.IsNullOrEmpty(zoneCode))
            {
                return;
            }
            if (db != null)
            {
                CurrentZone = db.CreateZoneWorkStation().Get(zoneCode);
                Contractors = db.CreateVirtualPersonStation<LandVirtualPerson>().GetByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
                Lands = db.CreateContractLandWorkstation().GetCollection(zoneCode, eLevelOption.SelfAndSubs);
                Concords = db.CreateConcordStation().GetByZoneCode(zoneCode);
                DotCollection = db.CreateBoundaryAddressDotWorkStation().GetByZoneCode(zoneCode, eSearchOption.Fuzzy, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                LineCollection = db.CreateBoundaryAddressCoilWorkStation().GetByZoneCode(zoneCode, eSearchOption.Fuzzy, eLandPropertyRightType.AgricultureLand);
                Books = db.CreateRegeditBookStation().GetByZoneCode(zoneCode, eSearchOption.Fuzzy);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Disponse()
        {
            Contractors = null;
            Lands = null;
            Concords = null;
            LandTypes = null;
            Tissues = null;
            DotCollection = null;
            LineCollection = null;
            Tables = null;
            GC.Collect();
        }

        #endregion
    }
}
