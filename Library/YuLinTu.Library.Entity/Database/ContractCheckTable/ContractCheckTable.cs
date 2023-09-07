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
    /// 农村土地承包经营权申请审批表
    /// </summary>
    [DataTable("ContractCheckTable")]
    [Serializable]
    public class ContractCheckTable : NameableObject  //(修改前)YltEntityIDName
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
		///审批表号

		/// </summary>
		[DataColumn("TableNumber", ColumnType = eDataType.String)]
		public string TableNumber { get; set; }
			
		/// <summary>
		///初审人意见

		/// </summary>
		[DataColumn("FirstCheckIdea", ColumnType = eDataType.String)]
		public string FirstCheckIdea { get; set; }
			
		/// <summary>
		///审查人

		/// </summary>
		[DataColumn("Checker", ColumnType = eDataType.String)]
		public string Checker { get; set; }
			
		/// <summary>
		///审查日期

		/// </summary>
		[DataColumn("CheckDate", ColumnType = eDataType.DateTime)]
		public DateTime? CheckDate { get; set; }
			
		/// <summary>
		///审核意见

		/// </summary>
		[DataColumn("CheckIdea", ColumnType = eDataType.String)]
		public string CheckIdea { get; set; }
			
		/// <summary>
		///审核人
		/// </summary>
		[DataColumn("Verifier", ColumnType = eDataType.String)]
		public string Verifier { get; set; }
			
		/// <summary>
		///审核日期
		/// </summary>
		[DataColumn("VerifyDate", ColumnType = eDataType.DateTime)]
		public DateTime? VerifyDate { get; set; }

		/// <summary>
		///公告日期
		/// </summary>
		[DataColumn("ParcardDate", ColumnType = eDataType.DateTime)]
		public DateTime? ParcardDate { get; set; }

		/// <summary>
		///公告结果
		/// </summary>
		[DataColumn("ParcardResult", ColumnType = eDataType.String)]
		public string ParcardResult { get; set; }
			
		/// <summary>
		///批准意见

		/// </summary>
		[DataColumn("PassIdea", ColumnType = eDataType.String)]
		public string PassIdea { get; set; }
			
		/// <summary>
		///审批人

		/// </summary>
		[DataColumn("FinalChecker", ColumnType = eDataType.String)]
		public string FinalChecker { get; set; }
			
		/// <summary>
		///审准日期

		/// </summary>
		[DataColumn("PassDate", ColumnType = eDataType.DateTime)]
		public DateTime? PassDate { get; set; }

		/// <summary>
		///领导批示

		/// </summary>
		[DataColumn("LeaderRecord", ColumnType = eDataType.String)]
		public string LeaderRecord { get; set; }
			
		/// <summary>
		///农村土地承包合同ID

		/// </summary>
		[DataColumn("ConcordID", ColumnType = eDataType.Guid)]
		public Guid ConcordID { get; set; }
			
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

        public ContractCheckTable()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            CheckDate = DateTime.Now;
            VerifyDate = DateTime.Now;
            ParcardDate = DateTime.Now;
            PassDate = DateTime.Now;
        }

        #endregion
	}
}