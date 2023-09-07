// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 水系水利设施
    /// </summary>
    [DataTable("CollectiveLand")]
    [Serializable]
    public class Irrigation : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private string name;
        private string projectNumber;
        private string useFor;
        public const string TableName = "Irrigation";

        #endregion

        #region Properties

        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataColumn("ID")]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        /// <summary>
        /// 地块名称
        /// </summary>
        [DataColumn("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 小地名
        /// </summary>
        [DataColumn("SmallName")]
        public string SmallName { get; set; }

        /// <summary>
        /// 工程名称
        /// </summary>
        [DataColumn("ProjectName")]
        public string ProjectName { get; set; }

        /// <summary>
        /// 调查编号
        /// </summary>
        [DataColumn("SurveyNumber")]
        public string SurveyNumber { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        [DataColumn("ProjectNumber")]
        public string ProjectNumber
        {
            get { return projectNumber; }
            set
            {
                projectNumber = value;
                NotifyPropertyChanged("ProjectNumber");
            }
        }

        /// <summary>
        /// 工程参数
        /// </summary>
        [DataColumn("ProjectAgrument")]
        public string ProjectAgrument { get; set; }

        /// <summary>
        /// 宗地统一编码
        /// </summary>
        [DataColumn("ParcelNumber")]
        public string ParcelNumber { get; set; }

        /// <summary>
        /// 所有权人ID（权属主体ID）。
        /// </summary>
        [DataColumn("OwnerID")]
        public Guid? OwnerID { get; set; }

        /// <summary>
        /// 承包方。
        /// </summary>
        [DataColumn("OwnerName")]
        public string OwnerName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [DataColumn("CartType")]
        public string CartType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [DataColumn("CartNumber")]
        public string CartNumber { get; set; }

        /// <summary>
        /// 工程地点（坐落）。
        /// </summary>
        [DataColumn("Location")]
        public string Location { get; set; }

        /// <summary>
        /// 工程占地（实测面积亩）
        /// </summary>
        [DataColumn("ActualArea")]
        public double ActualArea { get; set; }

        /// <summary>
        /// 建成时间。
        /// </summary>
        [DataColumn("RegisteDate")]
        public DateTime? RegisteDate { get; set; }

        /// <summary>
        /// 有效期限。
        /// </summary>
        [DataColumn("LandUseEndDate")]
        public DateTime? LandUseEndDate { get; set; }

        /// <summary>
        /// 所在地域。
        /// </summary>
        [DataColumn("ZoneCode")]
        public string ZoneCode { get; set; }

        /// <summary>
        /// 地域名称。
        /// </summary>
        [DataColumn("ZoneName")]
        public string ZoneName { get; set; }

        /// <summary>
        /// 工程用途。
        /// </summary>
        [DataColumn("UseFor")]
        public string UseFor
        {
            get { return useFor; }
            set
            {
                useFor = value;
                NotifyPropertyChanged("UseFor");
            }
        }

        /// <summary>
        /// 管理模式。
        /// </summary>
        [DataColumn("ManageModel")]
        public string ManageModel { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        [DataColumn("IrrigaeType")]
        public eIrrigationType IrrigaeType { get; set; }

        /// <summary>
        /// 地籍区代码
        /// </summary>
        [DataColumn("ParcelCode")]
        public string ParcelCode { get; set; }

        /// <summary>
        /// 地籍区名称
        /// </summary>
        [DataColumn("ParcelName")]
        public string ParcelName { get; set; }

        /// <summary>
        /// 四至
        /// </summary>
        [DataColumn("Neighbor")]
        public string Neighbor { get; set; }

        /// <summary>
        /// 调查人。
        /// </summary>
        [DataColumn("SurveyPerson")]
        public string SurveyPerson { get; set; }

        /// <summary>
        /// 调查日期。
        /// </summary>
        [DataColumn("SurveyDate")]
        public DateTime? SurveyDate { get; set; }

        /// <summary>
        /// 调查记事
        /// </summary>
        [DataColumn("SurveyOption")]
        public string SurveyOption { get; set; }

        /// <summary>
        /// 勘丈人。
        /// </summary>
        [DataColumn("Measurer")]
        public string Measurer { get; set; }

        /// <summary>
        /// 勘丈记事。
        /// </summary>
        [DataColumn("MeasureContent")]
        public string MeasureContent { get; set; }

        /// <summary>
        /// 勘丈日期。
        /// </summary>
        [DataColumn("MeasureDate")]
        public DateTime? MeasureDate { get; set; }

        /// <summary>
        /// 审核人。
        /// </summary>
        [DataColumn("Checker")]
        public string Checker { get; set; }

        /// <summary>
        /// 审核日期。
        /// </summary>
        [DataColumn("CheckDate")]
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        [DataColumn("CheckerOption")]
        public string CheckerOption { get; set; }

        /// <summary>
        /// 年号
        /// </summary>
        [DataColumn("Year")]
        public string Year { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [DataColumn("Number")]
        public string Number { get; set; }

        /// <summary>
        /// 发证机关
        /// </summary>
        [DataColumn("AwareOrgan")]
        public string AwareOrgan { get; set; }

        /// <summary>
        /// 发证日期
        /// </summary>
        [DataColumn("AwareDate")]
        public DateTime? AwareDate { get; set; }

        /// <summary>
        /// 共有人列表
        /// </summary>
        //public IrrigateSharedPerson SharePersonEntry { get; set; }

        /// <summary>
        /// 共有人列表
        /// </summary>
        [DataColumn("SharePerson")]
        public string SharePerson { get; set; }

        /// <summary>
        /// 水库
        /// </summary>
        //public Reservoir ReservoirEntry { get; set; }

        /// <summary>
        /// 水库扩展
        /// </summary>
        [DataColumn("ReservoirExpand")]
        public string ReservoirExpand { get; set; }

        /// <summary>
        /// 山平塘
        /// </summary>
        //public Mountain MountainEntry { get; set; }

        /// <summary>
        /// 山平塘扩展
        /// </summary>
        [DataColumn("MountainExpand")]
        public string MountainExpand { get; set; }

        /// <summary>
        /// 石河堰
        /// </summary>
        //public StoneWeir StoneWeirEntry { get; set; }

        /// <summary>
        /// 石河堰扩展
        /// </summary>
        [DataColumn("StoneWeirExpand")]
        public string StoneWeirExpand { get; set; }

        /// <summary>
        /// 泵站
        /// </summary>
        //public PumpStation PumpStationEntry { get; set; }

        /// <summary>
        /// 泵站扩展
        /// </summary>
        [DataColumn("PumpStationExpand")]
        public string PumpStationExpand { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        //public Channel ChannelEntry { get; set; }

        /// <summary>
        /// 渠道扩展
        /// </summary>
        [DataColumn("ChannelExpand")]
        public string ChannelExpand { get; set; }

        /// <summary>
        /// 蓄水池
        /// </summary>
        //public WaterReservoir WaterReservoirEntry { get; set; }

        /// <summary>
        /// 蓄水池扩展
        /// </summary>
        [DataColumn("WaterReservoirExpand")]
        public string WaterReservoirExpand { get; set; }

        /// <summary>
        /// 节水工程
        /// </summary>
        //public Conservation ConservationEntry { get; set; }

        /// <summary>
        /// 节水工程扩展
        /// </summary>
        [DataColumn("ConservationExpand")]
        public string ConservationExpand { get; set; }

        /// <summary>
        /// 提防工程
        /// </summary>
        //public BewareProject BewareProjectEntry { get; set; }

        /// <summary>
        /// 提防工程扩展
        /// </summary>
        [DataColumn("BewareProjectExpand")]
        public string BewareProjectExpand { get; set; }

        /// <summary>
        /// 供水工程
        /// </summary>
        //public WaterSupply WaterSupplyEntry { get; set; }

        /// <summary>
        /// 供水工程扩展
        /// </summary>
        [DataColumn("WaterSupplyExpand")]
        public string WaterSupplyExpand { get; set; }

        /// <summary>
        /// 扩展实体
        /// </summary>
        // public IrrigationExpand ExpandEntry { get; set; }

        /// <summary>
        /// 扩展
        /// </summary>
        [DataColumn("ExpandString")]
        public string ExpandString { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("Comment")]
        public string Comment { get; set; }

        #endregion

        #region Ctor

        public Irrigation()
        {
            IrrigaeType = eIrrigationType.Reservoir;
            UseFor = "人畜饮用水源";
            ManageModel = "单户";
            CartType = "身份证号码";
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化实体
        /// </summary>
        public void InitalizeDataValue()
        {
            //ReservoirEntry = new Reservoir();
            //MountainEntry = new Mountain();
            //BewareProjectEntry = new BewareProject();
            //ChannelEntry = new Channel();
            //ConservationEntry = new Conservation();
            //PumpStationEntry = new PumpStation();
            //StoneWeirEntry = new StoneWeir();
            //WaterReservoirEntry = new WaterReservoir();
            //WaterSupplyEntry = new WaterSupply();
            //SharePersonEntry = new IrrigateSharedPerson();
            //ExpandEntry = new IrrigationExpand();
        }

        /// <summary>
        /// 初始化数据扩展
        /// </summary>
        public void InitalizeDataExpand()
        {
            //if (ReservoirEntry != null)
            //{
            //    ReservoirExpand = ToolSerialization.SerializeXmlString(ReservoirEntry);
            //}
            //if (MountainEntry != null)
            //{
            //    MountainExpand = ToolSerialization.SerializeXmlString(MountainEntry);
            //}
            //if (BewareProjectEntry != null)
            //{
            //    BewareProjectExpand = ToolSerialization.SerializeXmlString(BewareProjectEntry);
            //}
            //if (ChannelEntry != null)
            //{
            //    ChannelExpand = ToolSerialization.SerializeXmlString(ChannelEntry);
            //}
            //if (ConservationEntry != null)
            //{
            //    ConservationExpand = ToolSerialization.SerializeXmlString(ConservationEntry);
            //}
            //if (PumpStationEntry != null)
            //{
            //    PumpStationExpand = ToolSerialization.SerializeXmlString(PumpStationEntry);
            //}
            //if (StoneWeirEntry != null)
            //{
            //    StoneWeirExpand = ToolSerialization.SerializeXmlString(StoneWeirEntry);
            //}
            //if (WaterReservoirEntry != null)
            //{
            //    WaterReservoirExpand = ToolSerialization.SerializeXmlString(WaterReservoirEntry);
            //}
            //if (WaterSupplyEntry != null)
            //{
            //    WaterSupplyExpand = ToolSerialization.SerializeXmlString(WaterSupplyEntry);
            //}
            //if (SharePersonEntry != null)
            //{
            //    SharePerson = SharePersonEntry.ToString();
            //}
            //if (ExpandEntry != null)
            //{
            //    ExpandString = ToolSerialization.SerializeXmlString(ExpandEntry);
            //}
        }

        /// <summary>
        /// 初始化数据实体
        /// </summary>
        public void InitalizeDataEntity()
        {
            //ReservoirEntry = string.IsNullOrEmpty(ReservoirExpand) ? new Reservoir() : (ToolSerialization.DeserializeXmlString(ReservoirExpand, typeof(Reservoir)) as Reservoir);
            //MountainEntry = string.IsNullOrEmpty(MountainExpand) ? new Mountain() : (ToolSerialization.DeserializeXmlString(MountainExpand, typeof(Mountain)) as Mountain);
            //BewareProjectEntry = string.IsNullOrEmpty(BewareProjectExpand) ? new BewareProject() : (ToolSerialization.DeserializeXmlString(BewareProjectExpand, typeof(BewareProject)) as BewareProject);
            //ChannelEntry = string.IsNullOrEmpty(ChannelExpand) ? new Channel() : (ToolSerialization.DeserializeXmlString(ChannelExpand, typeof(Channel)) as Channel);
            //ConservationEntry = string.IsNullOrEmpty(ConservationExpand) ? new Conservation() : (ToolSerialization.DeserializeXmlString(ConservationExpand, typeof(Conservation)) as Conservation);
            //PumpStationEntry = string.IsNullOrEmpty(PumpStationExpand) ? new PumpStation() : (ToolSerialization.DeserializeXmlString(PumpStationExpand, typeof(PumpStation)) as PumpStation);
            //StoneWeirEntry = string.IsNullOrEmpty(StoneWeirExpand) ? new StoneWeir() : (ToolSerialization.DeserializeXmlString(StoneWeirExpand, typeof(StoneWeir)) as StoneWeir);
            //WaterReservoirEntry = string.IsNullOrEmpty(WaterReservoirExpand) ? new WaterReservoir() : (ToolSerialization.DeserializeXmlString(WaterReservoirExpand, typeof(WaterReservoir)) as WaterReservoir);
            //WaterSupplyEntry = string.IsNullOrEmpty(WaterSupplyExpand) ? new WaterSupply() : (ToolSerialization.DeserializeXmlString(WaterSupplyExpand, typeof(WaterSupply)) as WaterSupply);
            //SharePersonEntry = string.IsNullOrEmpty(SharePerson) ? new IrrigateSharedPerson() : new IrrigateSharedPerson(SharePerson);
            //ExpandEntry = string.IsNullOrEmpty(ExpandString) ? new IrrigationExpand() : (ToolSerialization.DeserializeXmlString(ExpandString, typeof(IrrigationExpand)) as IrrigationExpand);
        }

        #endregion
    }
}
