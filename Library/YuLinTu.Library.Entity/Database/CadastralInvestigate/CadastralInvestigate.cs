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
    /// 地籍调查表
    /// </summary>
    [DataTable("CadastralInvestigate")]
    [Serializable]
    public class CadastralInvestigate : NameableObject
    {
        #region Ctor

        static CadastralInvestigate()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandUsePersonType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eCredentialsType);
        }

        #endregion

        #region Fields

        private string _ZoneCode = string.Empty;

        #endregion

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
        ///上级管理部门
        /// </summary>
        [DataColumn("Administration", ColumnType = eDataType.String)]
        public string Administration { get; set; }

        /// <summary>
        ///土地使用者性质
        /// </summary>
        [DataColumn("LandUsePersonType", ColumnType = eDataType.Int32)]
        public eLandUsePersonType LandUsePersonType { get; set; }

        /// <summary>
        ///预编地籍号
        /// </summary>
        [DataColumn("PreCadastralNumber", ColumnType = eDataType.String)]
        public string PreCadastralNumber { get; set; }

        /// <summary>
        ///地籍号
        /// </summary>
        [DataColumn("CadastralNumber", ColumnType = eDataType.String)]
        public string CadastralNumber { get; set; }

        /// <summary>
        ///所在图幅号
        /// </summary>
        [DataColumn("ImageNumber", ColumnType = eDataType.String)]
        public string ImageNumber { get; set; }

        /// <summary>
        ///批准用途
        /// </summary>
        [DataColumn("ApprovePurpose", ColumnType = eDataType.String)]
        public string ApprovePurpose { get; set; }

        /// <summary>
        ///实际用途
        /// </summary>
        [DataColumn("RealPurpose", ColumnType = eDataType.String)]
        public string RealPurpose { get; set; }

        /// <summary>
        ///共有使用权情况
        /// </summary>
        [DataColumn("ShareUsage", ColumnType = eDataType.String)]
        public string ShareUsage { get; set; }

        /// <summary>
        ///说明
        /// </summary>
        [DataColumn("Comment", ColumnType = eDataType.String)]
        public string Comment { get; set; }

        /// <summary>
        ///土地证编号
        /// </summary>
        [DataColumn("CertificateNumber", ColumnType = eDataType.String)]
        public string CertificateNumber { get; set; }

        /// <summary>
        ///使用期限
        /// </summary>
        [DataColumn("UseDate", ColumnType = eDataType.String)]
        public string UseDate { get; set; }

        /// <summary>
        ///所在地域
        /// </summary>
        [DataColumn("ZoneCode", ColumnType = eDataType.String)]
        public string ZoneCode
        {
            get { return _ZoneCode; }
            set
            {
                _ZoneCode = value;
                if (!string.IsNullOrEmpty(_ZoneCode))
                    _ZoneCode = _ZoneCode.Trim();
            }
        }

        /// <summary>
        ///法人代表姓名
        /// </summary>
        [DataColumn("RepresentName", ColumnType = eDataType.String)]
        public string RepresentName { get; set; }

        /// <summary>
        ///法人代表证件类型
        /// </summary>
        [DataColumn("RepresentType", ColumnType = eDataType.Int32)]
        public eCredentialsType RepresentType { get; set; }

        /// <summary>
        ///法人代表证件号
        /// </summary>
        [DataColumn("RepresentNumber", ColumnType = eDataType.String)]
        public string RepresentNumber { get; set; }

        /// <summary>
        ///法人代表身份证明书
        /// </summary>
        [DataColumn("RepresentProveBook", ColumnType = eDataType.String)]
        public string RepresentProveBook { get; set; }

        /// <summary>
        ///法人代表电话号码
        /// </summary>
        [DataColumn("RepresentTelphone", ColumnType = eDataType.String)]
        public string RepresentTelphone { get; set; }

        /// <summary>
        ///代理人姓名
        /// </summary>
        [DataColumn("AgentName", ColumnType = eDataType.String)]
        public string AgentName { get; set; }

        /// <summary>
        ///代理人证件类型
        /// </summary>
        [DataColumn("AgentCrdentialType", ColumnType = eDataType.Int32)]
        public eCredentialsType AgentCrdentialType { get; set; }

        /// <summary>
        ///代理人证件号
        /// </summary>
        [DataColumn("AgentCrdentialNumber", ColumnType = eDataType.String)]
        public string AgentCrdentialNumber { get; set; }

        /// <summary>
        ///代理人身份证明书
        /// </summary>
        [DataColumn("AgentProveBook", ColumnType = eDataType.String)]
        public string AgentProveBook { get; set; }

        /// <summary>
        ///代理人电话号码
        /// </summary>
        [DataColumn("AgentTelphone", ColumnType = eDataType.String)]
        public string AgentTelphone { get; set; }

        /// <summary>
        ///调查表编号
        /// </summary>
        [DataColumn("Number", ColumnType = eDataType.String)]
        public string Number { get; set; }

        /// <summary>
        ///调查表填写日期
        /// </summary>
        [DataColumn("WriteDate", ColumnType = eDataType.DateTime)]
        public DateTime WriteDate { get; set; }

        /// <summary>
        ///界址调查员姓名
        /// </summary>
        [DataColumn("PersonName", ColumnType = eDataType.String)]
        public string PersonName { get; set; }

        /// <summary>
        ///地籍调查表附件路径
        /// </summary>
        [DataColumn("Path", ColumnType = eDataType.String)]
        public string Path { get; set; }

        /// <summary>
        ///扩展属性A 备用
        /// </summary>
        [DataColumn("ExtendA", ColumnType = eDataType.String)]
        public string ExtendA { get; set; }

        /// <summary>
        ///扩展属性B 备用
        /// </summary>
        [DataColumn("ExtendB", ColumnType = eDataType.String)]
        public string ExtendB { get; set; }

        /// <summary>
        ///扩展属性C 备用
        /// </summary>
        [DataColumn("ExtendC", ColumnType = eDataType.String)]
        public string ExtendC { get; set; }

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
        ///批准日期
        /// </summary>
        [DataColumn("ApproveDate", ColumnType = eDataType.DateTime)]
        public DateTime? ApproveDate { get; set; }

        /// <summary>
        ///终止日期
        /// </summary>
        [DataColumn("OverDate", ColumnType = eDataType.DateTime)]
        public DateTime? OverDate { get; set; }

        /// <summary>
        ///权属调查记事及调查员意见
        /// </summary>
        [DataColumn("DeliveranceComment", ColumnType = eDataType.String)]
        public string DeliveranceComment { get; set; }

        /// <summary>
        ///地籍堪丈记事
        /// </summary>
        [DataColumn("AdversariaComment", ColumnType = eDataType.String)]
        public string AdversariaComment { get; set; }

        /// <summary>
        ///地籍调查结果审核意见
        /// </summary>
        [DataColumn("AuditingComment", ColumnType = eDataType.String)]
        public string AuditingComment { get; set; }

        /// <summary>
        ///变更简要说明
        /// </summary>
        [DataColumn("ChangSimpleComment", ColumnType = eDataType.String)]
        public string ChangSimpleComment { get; set; }

        /// <summary>
        ///申请登记依据
        /// </summary>
        [DataColumn("ApplyAttest", ColumnType = eDataType.String)]
        public string ApplyAttest { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-调查员姓名
        /// </summary>
        [DataColumn("DeliveranceUser", ColumnType = eDataType.String)]
        public string DeliveranceUser { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-日期
        /// </summary>
        [DataColumn("DeliveranceDate", ColumnType = eDataType.DateTime)]
        public DateTime? DeliveranceDate { get; set; }

        /// <summary>
        ///地籍堪丈记事-调查员姓名
        /// </summary>
        [DataColumn("AdversariaUser", ColumnType = eDataType.String)]
        public string AdversariaUser { get; set; }

        /// <summary>
        ///地籍堪丈记事-日期
        /// </summary>
        [DataColumn("AdversariaDate", ColumnType = eDataType.DateTime)]
        public DateTime? AdversariaDate { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-调查员姓名
        /// </summary>
        [DataColumn("AuditingUser", ColumnType = eDataType.String)]
        public string AuditingUser { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-日期
        /// </summary>
        [DataColumn("AuditingDate", ColumnType = eDataType.DateTime)]
        public DateTime? AuditingDate { get; set; }

        #endregion

        #region Ctor

        public CadastralInvestigate()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            this.RealPurpose = "072";
            this.ApprovePurpose = "072";
            LandUsePersonType = eLandUsePersonType.Individual;
            RepresentType = eCredentialsType.IdentifyCard;
            AgentCrdentialType = eCredentialsType.IdentifyCard;
        }

        #endregion
    }
}