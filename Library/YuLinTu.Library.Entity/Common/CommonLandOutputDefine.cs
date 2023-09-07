using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地块自定义
    /// </summary>
    [Serializable]
    public class CommonLandOutputDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 所有权人索引
        /// </summary>
        public bool NameValue { get; set; }

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        public bool NumberValue { get; set; }

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        public bool NumberNameValue { get; set; }

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        public bool NumberGenderValue { get; set; }

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        public bool NumberAgeValue { get; set; }

        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        public bool NumberIcnValue { get; set; }

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        public bool NumberReleationValue { get; set; }

        /// <summary>
        /// 家庭成员备注索引
        /// </summary>
        public bool NumberCommentValue { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public bool NatureValue { get; set; }

        /// <summary>
        /// 土地所有者
        /// </summary>
        public bool LandOwnerValue { get; set; }

        /// <summary>
        /// 土地座落
        /// </summary>
        public bool LandLocationValue { get; set; }

        /// <summary>
        /// 上级主管部门
        /// </summary>
        public bool DepartmentValue { get; set; }

        /// <summary>
        /// 法人代表姓名
        /// </summary>
        public bool RepresentNameValue { get; set; }

        /// <summary>
        /// 法人代表证件号
        /// </summary>
        public bool RepresentNumberValue { get; set; }

        /// <summary>
        /// 法人代表电话号码
        /// </summary>
        public bool RepresentTelphoneValue { get; set; }

        /// <summary>
        /// 代理人姓名
        /// </summary>
        public bool AgentNameValue { get; set; }

        /// <summary>
        /// 代理人证件号
        /// </summary>
        public bool AgentCrdentialNumberValue { get; set; }

        /// <summary>
        /// 代理人电话号码
        /// </summary>
        public bool AgentTelphoneValue { get; set; }

        /// <summary>
        /// 土地权属性质
        /// </summary>
        public bool OwnRightTypeValue { get; set; }

        /// <summary>
        /// 地籍号
        /// </summary>
        public bool LandNumberValue { get; set; }

        /// <summary>
        /// 图号
        /// </summary>
        public bool ImageNumberValue { get; set; }

        /// <summary>
        /// 四至东索引
        /// </summary>
        public bool LandNeighborValue { get; set; }

        /// <summary>
        /// 实测面积
        /// </summary>
        public bool ActualAreaValue { get; set; }

        /// <summary>
        /// 确权面积
        /// </summary>
        public bool AwareAreaValue { get; set; }

        /// <summary>
        /// 超占面积
        /// </summary>
        public bool ExceedAreaValue { get; set; }

        /// <summary>
        /// 违法面积
        /// </summary>
        public bool IllegalAreaValue { get; set; }

        /// <summary>
        /// 地类
        /// </summary>
        public bool LandTypeValue { get; set; }

        /// <summary>
        /// 批准用途
        /// </summary>
        public bool ApprovePurposeValue { get; set; }

        /// <summary>
        /// 批准日期
        /// </summary>
        public bool ApproveDateValue { get; set; }

        /// <summary>
        /// 实际用途
        /// </summary>
        public bool RealPurposeValue { get; set; }

        /// <summary>
        /// 使用权类型
        /// </summary>
        public bool UseRightTypeValue { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        public bool UseDateValue { get; set; }

        /// <summary>
        /// 变更简要说明
        /// </summary>
        public bool ChangSimpleCommentValue { get; set; }

        /// <summary>
        ///申请登记依据
        /// </summary>
        public bool ApplyAttestValue { get; set; }

        /// <summary>
        ///权属调查记事及调查员意见
        /// </summary>
        public bool DeliveranceCommentValue { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-调查员姓名
        /// </summary>
        public bool DeliveranceUserValue { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-日期
        /// </summary>
        public bool DeliveranceDateValue { get; set; }

        /// <summary>
        ///地籍堪丈记事
        /// </summary>
        public bool AdversariaCommentValue { get; set; }

        /// <summary>
        ///地籍堪丈记事-调查员姓名
        /// </summary>
        public bool AdversariaUserValue { get; set; }

        /// <summary>
        ///地籍堪丈记事-日期
        /// </summary>
        public bool AdversariaDateValue { get; set; }

        /// <summary>
        ///地籍调查结果审核意见
        /// </summary>
        public bool AuditingCommentValue { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-调查员姓名
        /// </summary>
        public bool AuditingUserValue { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-日期
        /// </summary>
        public bool AuditingDateValue { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        public bool CommentValue { get; set; }

        /// <summary>
        ///权证编号
        /// </summary>
        public bool WarrantNumberValue { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount { get; set; }

        #endregion

        #region Ctor

        public CommonLandOutputDefine()
        {
            NameValue = true;
            NumberValue = true;
            NumberNameValue = true;
            NumberGenderValue = true;
            NumberAgeValue = true;
            NumberIcnValue = true;
            NumberReleationValue = false;
            NumberCommentValue = false;
            NatureValue = true;
            LandOwnerValue = true;
            LandLocationValue = true;
            DepartmentValue = true;
            RepresentNameValue = true;
            RepresentNumberValue = true;
            RepresentTelphoneValue = true;
            AgentNameValue = true;
            AgentCrdentialNumberValue = true;
            AgentTelphoneValue = true;
            OwnRightTypeValue = true;
            LandNumberValue = true;
            ImageNumberValue = true;
            LandNeighborValue = true;
            ActualAreaValue = true;
            AwareAreaValue = true;
            ExceedAreaValue = true;
            IllegalAreaValue = true;
            LandTypeValue = true;
            ApprovePurposeValue = true;
            ApproveDateValue = true;
            RealPurposeValue = true;
            UseRightTypeValue = true;
            UseDateValue = true;
            ChangSimpleCommentValue = true;
            ApplyAttestValue = true;
            DeliveranceCommentValue = true;
            DeliveranceUserValue = true;
            DeliveranceDateValue = true;
            AdversariaCommentValue = true;
            AdversariaUserValue = true;
            AdversariaDateValue = true;
            AuditingCommentValue = true;
            AuditingUserValue = true;
            AuditingDateValue = true;
            CommentValue = true;
            WarrantNumberValue = true;
            ColumnCount = 47;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化建设用地值
        /// </summary>
        public void InitalizeConsValue()
        {
            NumberValue = false;
            NumberNameValue = false;
            NumberGenderValue = false;
            NumberAgeValue = false;
            NumberIcnValue = false;
            ColumnCount = 41;
        }

        #endregion
    }

    public class CommonLandOutputDefineCollection //(修改前): YltEntityCollection<CommonLandOutputDefine>
    {
    }
}
