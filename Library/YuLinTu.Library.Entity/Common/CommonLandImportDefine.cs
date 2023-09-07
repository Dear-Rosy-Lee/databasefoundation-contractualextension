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
    public class CommonLandImportDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 所有权人索引
        /// </summary>
        public int NameIndex { get; set; }

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        public int NumberIndex { get; set; }

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        public int NumberNameIndex { get; set; }

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        public int NumberGenderIndex { get; set; }

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        public int NumberAgeIndex { get; set; }

        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        public int NumberIcnIndex { get; set; }

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        public int NumberReleationIndex { get; set; }

        /// <summary>
        /// 家庭成员备注索引
        /// </summary>
        public int NumberCommentIndex { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public int NatureIndex { get; set; }

        /// <summary>
        /// 土地所有者
        /// </summary>
        public int LandOwnerIndex { get; set; }

        /// <summary>
        /// 土地座落
        /// </summary>
        public int LandLocationIndex { get; set; }

        /// <summary>
        /// 上级主管部门
        /// </summary>
        public int DepartmentIndex { get; set; }

        /// <summary>
        /// 法人代表姓名
        /// </summary>
        public int RepresentNameIndex { get; set; }

        /// <summary>
        /// 法人代表证件号
        /// </summary>
        public int RepresentNumberIndex { get; set; }

        /// <summary>
        /// 法人代表电话号码
        /// </summary>
        public int RepresentTelphoneIndex { get; set; }

        /// <summary>
        /// 代理人姓名
        /// </summary>
        public int AgentNameIndex { get; set; }

        /// <summary>
        /// 代理人证件号
        /// </summary>
        public int AgentCrdentialNumberIndex { get; set; }

        /// <summary>
        /// 代理人电话号码
        /// </summary>
        public int AgentTelphoneIndex { get; set; }

        /// <summary>
        /// 土地权属性质
        /// </summary>
        public int OwnRightTypeIndex { get; set; }

        /// <summary>
        /// 地籍号
        /// </summary>
        public int LandNumberIndex { get; set; }

        /// <summary>
        /// 图号
        /// </summary>
        public int ImageNumberIndex { get; set; }

        /// <summary>
        /// 四至东索引
        /// </summary>
        public int EastIndex { get; set; }

        /// <summary>
        /// 四至南索引
        /// </summary>
        public int SourthIndex { get; set; }

        /// <summary>
        /// 四至西索引
        /// </summary>
        public int WestIndex { get; set; }

        /// <summary>
        /// 四至北索引
        /// </summary>
        public int NorthIndex { get; set; }

        /// <summary>
        /// 实测面积
        /// </summary>
        public int ActualAreaIndex { get; set; }

        /// <summary>
        /// 确权面积
        /// </summary>
        public int AwareAreaIndex { get; set; }

        /// <summary>
        /// 超占面积
        /// </summary>
        public int ExceedAreaIndex { get; set; }

        /// <summary>
        /// 违法面积
        /// </summary>
        public int IllegalAreaIndex { get; set; }

        /// <summary>
        /// 地类
        /// </summary>
        public int LandTypeIndex { get; set; }

        /// <summary>
        /// 批准用途
        /// </summary>
        public int ApprovePurposeIndex { get; set; }

        /// <summary>
        /// 批准日期
        /// </summary>
        public int ApproveDateIndex { get; set; }

        /// <summary>
        /// 实际用途
        /// </summary>
        public int RealPurposeIndex { get; set; }

        /// <summary>
        /// 使用权类型
        /// </summary>
        public int UseRightTypeIndex { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        public int UseDateIndex { get; set; }

        /// <summary>
        /// 变更简要说明
        /// </summary>
        public int ChangSimpleCommentIndex { get; set; }

        /// <summary>
        ///申请登记依据
        /// </summary>
        public int ApplyAttestIndex { get; set; }

        /// <summary>
        ///权属调查记事及调查员意见
        /// </summary>
        public int DeliveranceCommentIndex { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-调查员姓名
        /// </summary>
        public int DeliveranceUserIndex { get; set; }

        /// <summary>
        ///权属调查记事及调查员情况-日期
        /// </summary>
        public int DeliveranceDateIndex { get; set; }

        /// <summary>
        ///地籍堪丈记事
        /// </summary>
        public int AdversariaCommentIndex { get; set; }

        /// <summary>
        ///地籍堪丈记事-调查员姓名
        /// </summary>
        public int AdversariaUserIndex { get; set; }

        /// <summary>
        ///地籍堪丈记事-日期
        /// </summary>
        public int AdversariaDateIndex { get; set; }

        /// <summary>
        ///地籍调查结果审核意见
        /// </summary>
        public int AuditingCommentIndex { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-调查员姓名
        /// </summary>
        public int AuditingUserIndex { get; set; }

        /// <summary>
        ///地籍调查结果审核意见-日期
        /// </summary>
        public int AuditingDateIndex { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        public int CommentIndex { get; set; }

        /// <summary>
        ///权证编号
        /// </summary>
        public int WarrantNumberIndex { get; set; }

        #endregion

        #region Ctor

        public CommonLandImportDefine()
        {
            NameIndex = 1;
            NumberIndex = 2;
            NumberNameIndex = 3;
            NumberGenderIndex = 4;
            NumberAgeIndex = 5;
            NumberIcnIndex = 6;
            NumberReleationIndex = -1;
            NumberCommentIndex = -1;
            NatureIndex = 7;
            LandOwnerIndex = 8;
            LandLocationIndex = 9;
            DepartmentIndex = 10;
            RepresentNameIndex = 11;
            RepresentNumberIndex = 12;
            RepresentTelphoneIndex = 13;
            AgentNameIndex = 14;
            AgentCrdentialNumberIndex = 15;
            AgentTelphoneIndex = 16;
            OwnRightTypeIndex = 17;
            LandNumberIndex = 18;
            ImageNumberIndex = 19;
            EastIndex = 20;
            SourthIndex = 21;
            WestIndex = 22;
            NorthIndex = 23;
            ActualAreaIndex = 24;
            AwareAreaIndex = 25;
            ExceedAreaIndex = 26;
            IllegalAreaIndex = 27;
            LandTypeIndex = 28;
            ApprovePurposeIndex = 29;
            ApproveDateIndex = 30;
            RealPurposeIndex = 31;
            UseRightTypeIndex = 32;
            UseDateIndex = 33;
            ChangSimpleCommentIndex = 34;
            ApplyAttestIndex = 35;
            DeliveranceCommentIndex = 36;
            DeliveranceUserIndex = 37;
            DeliveranceDateIndex = 38;
            AdversariaCommentIndex = 39;
            AdversariaUserIndex = 40;
            AdversariaDateIndex = 41;
            AuditingCommentIndex = 42;
            AuditingUserIndex = 43;
            AuditingDateIndex = 44;
            CommentIndex = 45;
            WarrantNumberIndex = 46;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化建设用地值
        /// </summary>
        public void InitalizeConsValue()
        {
            NameIndex = 1;
            NumberIndex = -1;
            NumberNameIndex = -1;
            NumberGenderIndex = -1;
            NumberAgeIndex = -1;
            NumberIcnIndex = -1;
            NatureIndex = 2;
            LandOwnerIndex = 3;
            LandLocationIndex = 4;
            DepartmentIndex = 5;
            RepresentNameIndex = 6;
            RepresentNumberIndex = 7;
            RepresentTelphoneIndex = 8;
            AgentNameIndex = 9;
            AgentCrdentialNumberIndex = 10;
            AgentTelphoneIndex = 11;
            OwnRightTypeIndex = 12;
            LandNumberIndex = 13;
            ImageNumberIndex = 14;
            EastIndex = 15;
            SourthIndex = 16;
            WestIndex = 17;
            NorthIndex = 18;
            ActualAreaIndex = 19;
            AwareAreaIndex = 20;
            ExceedAreaIndex = 21;
            IllegalAreaIndex = 22;
            LandTypeIndex = 23;
            ApprovePurposeIndex = 24;
            ApproveDateIndex = 25;
            RealPurposeIndex = 26;
            UseRightTypeIndex = 27;
            UseDateIndex = 28;
            ChangSimpleCommentIndex = 29;
            ApplyAttestIndex = 30;
            DeliveranceCommentIndex = 31;
            DeliveranceUserIndex = 32;
            DeliveranceDateIndex = 33;
            AdversariaCommentIndex = 34;
            AdversariaUserIndex = 35;
            AdversariaDateIndex = 36;
            AuditingCommentIndex = 37;
            AuditingUserIndex = 38;
            AuditingDateIndex = 39;
            CommentIndex = 40;
            WarrantNumberIndex = 41;
        }

        #endregion
    }

    public class CommonLandImportDefineCollection //(修改前): YltEntityCollection<CommonLandImportDefine>
    {
    }
}
