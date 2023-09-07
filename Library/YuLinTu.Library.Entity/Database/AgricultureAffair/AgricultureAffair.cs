// (C) 2014 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包经营权业务记录
    /// </summary>
    [DataTable("AgricultureAffair")]
    [Serializable]
    public partial class AgricultureAffair : NameableObject
    {
        #region ConstName

        public const string TABLENAME = "AgricultureAffair";//表名称

        #endregion

        #region Fields

        #endregion

        #region Properties
        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataColumn("ID", ColumnType = eDataType.Guid)]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        [DataColumn("Name", ColumnType = eDataType.String)]
        public new string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 事务类型
        /// </summary>
        [DataColumn("Category", ColumnType = eDataType.Int32)]
        public eTaskType Category { get; set; }

        /// <summary>
        /// 权属类型
        /// </summary>
        [DataColumn("OwnershipCategory", ColumnType = eDataType.Int32)]
        public ePropertyRightType OwnershipCategory { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [DataColumn("Description", ColumnType = eDataType.String)]
        public string Description { get; set; }

        /// <summary>
        /// 任务工作参数
        /// </summary>
        [DataColumn("WorkAgrument", ColumnType = eDataType.String)]
        public string WorkAgrument { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [DataColumn("Status", ColumnType = eDataType.Int32)]
        public eProgressType Status { get; set; }

        /// <summary>
        /// 所在地域
        /// </summary>
        [DataColumn("ZoneCode", ColumnType = eDataType.String)]
        public string ZoneCode { get; set; }

        /// <summary>
        /// 所在地域名称
        /// </summary>
        [DataColumn("ZoneName", ColumnType = eDataType.String)]
        public string ZoneName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataColumn("CreaterTime", ColumnType = eDataType.DateTime)]
        public DateTime? CreaterTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataColumn("ModifyerTime", ColumnType = eDataType.DateTime)]
        public DateTime? ModifyerTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataColumn("Creater", ColumnType = eDataType.String)]
        public string Creater { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        [DataColumn("Modifyer", ColumnType = eDataType.String)]
        public string Modifyer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("Comment", ColumnType = eDataType.String)]
        public string Comment { get; set; }

        /// <summary>
        /// 扩展集合
        /// </summary>
        public List<AgricultureAffairExpand> ExpandCollection
        {
            get
            {
                //if (string.IsNullOrEmpty(WorkAgrument))
                //{
                //    return new List<AgricultureAffairExpand>();
                //}
                //return ToolSerialization.DeserializeXmlString(WorkAgrument, typeof(List<AgricultureAffairExpand>)) as List<AgricultureAffairExpand>;
            throw new NotImplementedException();
            } 
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get
            {
                //int count = 0;
                //if (string.IsNullOrEmpty(WorkAgrument))
                //{
                //    return 0;
                //}
                //List<AgricultureAffairExpand> expandCollection = ToolSerialization.DeserializeXmlString(WorkAgrument, typeof(List<AgricultureAffairExpand>)) as List<AgricultureAffairExpand>;
                //count = expandCollection.Count;
                //return count;
            throw new NotImplementedException();
            }
        }

        #endregion

        #region Ctor

        public AgricultureAffair()
        {
            Category = eTaskType.InitializationRegister;
            OwnershipCategory = ePropertyRightType.ContractLand;
            Status = eProgressType.Unknown;
        }

        #endregion
    }
}
