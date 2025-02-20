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
    /// 共有人
    /// </summary>
    [DataTable("SharePerson")]
    [Serializable]
    public class SharePerson : NameableObject
    {
        #region Ctor

        static SharePerson()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eCredentialsType);
        }

        #endregion

        #region Properties

        /// <summary>
		///标识
		/// </summary>
		[DataColumn("ID", ColumnType = eDataType.Guid)]
		public Guid ID
		{
			get;
			set;
		}
			
		/// <summary>
		///姓名

		/// </summary>
		[DataColumn("Name", ColumnType = eDataType.String)]
		public new string Name
		{
			get {return base.Name;}
			set {base.Name = value;}
		}
			
		/// <summary>
		///性别
		/// </summary>
		[DataColumn("Sex", ColumnType = eDataType.Int32)]
		public int Sex { get; set; }
			
		/// <summary>
		///年龄
		/// </summary>
		[DataColumn("Age", ColumnType = eDataType.Int32)]
		public int Age { get; set; }
			
		/// <summary>
		///证件类型
		/// </summary>
		[DataColumn("CredentialType", ColumnType = eDataType.Int32)]
		public eCredentialsType CredentialType { get; set; }
			
		/// <summary>
		///证件编号

		/// </summary>
		[DataColumn("CredentialNumber", ColumnType = eDataType.String)]
		public string CredentialNumber { get; set; }
			
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
		public DateTime CreationTime { get; set; }
			
		/// <summary>
		///最后修改者

		/// </summary>
		[DataColumn("Modifier", ColumnType = eDataType.String)]
		public string Modifier { get; set; }
			
		/// <summary>
		///最后修改时间

		/// </summary>
		[DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
		public DateTime ModifiedTime { get; set; }
			
		/// <summary>
		///备注
		/// </summary>
		[DataColumn("Comment", ColumnType = eDataType.String)]
		public string Comment { get; set; }
			
		#endregion

        #region Ctor

        public SharePerson()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            CredentialType = eCredentialsType.Other;
        }

        #endregion
	}
}