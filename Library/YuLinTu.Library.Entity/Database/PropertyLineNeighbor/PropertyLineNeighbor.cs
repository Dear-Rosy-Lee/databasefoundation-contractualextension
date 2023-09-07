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
    /// 界址线四至
    /// </summary>
    [DataTable("PropertyLineNeighbor")]
    [Serializable]
    public class PropertyLineNeighbor : NameableObject
    {
        #region Properties
        
		/// <summary>
		///
		/// </summary>
		[DataColumn("ID", ColumnType = eDataType.Guid)]
		public Guid ID
		{
			get;
			set;
		}
			
		/// <summary>
		///集体建设用地使用权ID
		/// </summary>
		[DataColumn("LandID", ColumnType = eDataType.Guid)]
		public Guid LandID { get; set; }
			
		/// <summary>
		///起点号
		/// </summary>
		[DataColumn("StartPoint", ColumnType = eDataType.Int32)]
		public int StartPoint { get; set; }
			
		/// <summary>
		///终点号
		/// </summary>
		[DataColumn("EndPoint", ColumnType = eDataType.Int32)]
		public int EndPoint { get; set; }
			
		/// <summary>
		///邻地指界人姓名
		/// </summary>
		[DataColumn("NeighborName", ColumnType = eDataType.String)]
		public string NeighborName { get; set; }
			
		/// <summary>
		///邻地地籍编号
		/// </summary>
		[DataColumn("NeighborCadastralNumber", ColumnType = eDataType.String)]
        public string NeighborCadastralNumber { get; set; }

        /// <summary>
        ///邻宗地承包方
        /// </summary>
        [DataColumn("NeighborPersonName", ColumnType = eDataType.String)]
        public string NeighborPersonName { get; set; }
			
		/// <summary>
		///本宗地指界人姓名
		/// </summary>
		[DataColumn("CurrentName", ColumnType = eDataType.String)]
		public string CurrentName { get; set; }
			
		/// <summary>
		///登记日期
		/// </summary>
		[DataColumn("RegistTime", ColumnType = eDataType.DateTime)]
		public DateTime? RegistTime { get; set; }
			
		/// <summary>
		///创建者
		/// </summary>
		[DataColumn("Founder", ColumnType = eDataType.String)]
		public string Founder { get; set; }
			
		/// <summary>
		///创建时间
		/// </summary>
		[DataColumn("CreationTime", ColumnType = eDataType.DateTime)]
		public DateTime? CreationTime { get; set; }
			
		/// <summary>
		///最后修改者
		/// </summary>
		[DataColumn("Modifier", ColumnType = eDataType.String)]
		public string Modifier { get; set; }
			
		/// <summary>
		///最后修改时间
		/// </summary>
		[DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
		public DateTime? ModifiedTime { get; set; }
			
		/// <summary>
		///备注
		/// </summary>
		[DataColumn("Comment", ColumnType = eDataType.String)]
		public string Comment { get; set; }

        #endregion

        #region Methods Compare

        public override int CompareTo(object obj)
        {
            PropertyLineNeighbor neighbor = obj as PropertyLineNeighbor;

            if (this.StartPoint > neighbor.StartPoint)
                return 1;
            else if (this.StartPoint < neighbor.StartPoint)
                return -1;

            return 0;
        }

        #endregion

        #region Ctor

        public PropertyLineNeighbor()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            RegistTime = DateTime.Now;
        }

        #endregion
	}
}