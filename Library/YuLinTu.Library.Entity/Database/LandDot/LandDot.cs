// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址点表
    /// </summary>
    [DataTable("LandDot")]
    [Serializable]
    public class LandDot : NameableObject
    {
        #region Properties
        
		/// <summary>
		///标识码
		/// </summary>
		[DataColumn("ID", ColumnType = eDataType.Guid)]
		public Guid ID
		{
			get;
			set;
		}
			
		/// <summary>
		///界址点号
		/// </summary>
		[DataColumn("DotNumber", ColumnType = eDataType.String)]
		public string DotNumber { get; set; }

        /// <summary>
        ///编号
        /// </summary>
        [DataColumn("Number", ColumnType = eDataType.Int32)]
        public int Number { get; set; }
			
		/// <summary>
		///界址点界桩种类
		/// </summary>
		[DataColumn("DotType", ColumnType = eDataType.String)]
		public string DotType { get; set; }
			
		/// <summary>
		///X坐标
		/// </summary>
		[DataColumn("XCoordinate", ColumnType = eDataType.Decimal)]
        public double XCoordinate { get; set; }
			
		/// <summary>
		///Y坐标
		/// </summary>
		[DataColumn("YCoordinate", ColumnType = eDataType.Decimal)]
        public double YCoordinate { get; set; }
			
		/// <summary>
		///界址线类型
		/// </summary>
		[DataColumn("CoilType", ColumnType = eDataType.String)]
		public string CoilType { get; set; }
			
		/// <summary>
		///界址线位置
		/// </summary>
		[DataColumn("CoilLocation", ColumnType = eDataType.String)]
		public string CoilLocation { get; set; }
			
		/// <summary>
		///本宗地指界人姓名
		/// </summary>
		[DataColumn("CurrentName", ColumnType = eDataType.String)]
		public string CurrentName { get; set; }
			
		/// <summary>
		///邻宗地指界人姓名
		/// </summary>
		[DataColumn("NeighborName", ColumnType = eDataType.String)]
		public string NeighborName { get; set; }
			
		/// <summary>
		///所在地域
		/// </summary>
		[DataColumn("ZoneCode", ColumnType = eDataType.String)]
		public string ZoneCode { get; set; }
			
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
		
		#region Ctor
		
		public LandDot()
		{
			CreationTime = DateTime.Now;
			ModifiedTime = DateTime.Now;
		}
		
		#endregion
	}
}