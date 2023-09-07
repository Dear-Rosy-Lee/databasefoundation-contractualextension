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
    public class CollectiveLandOutputDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 所有权人索引
        /// </summary>
        public bool NameValue { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public bool NatureValue { get; set; }

        /// <summary>
        /// 上级主管部门
        /// </summary>
        public bool DepartmentValue { get; set; }

        /// <summary>
        /// 土地座落
        /// </summary>
        public bool LandLocationValue { get; set; }

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
        /// 权属单位代码
        /// </summary>
        public bool OwnUnitCodeValue { get; set; }

        /// <summary>
        /// 宗地号
        /// </summary>
        public bool LandNumberValue { get; set; }

        /// <summary>
        /// 图号
        /// </summary>
        public bool ImageNumberValue { get; set; }

        /// <summary>
        /// 总面积
        /// </summary>
        public bool AreaValue { get; set; }

        /// <summary>
        /// 农用地面积
        /// </summary>
        public bool FarmerLandAreaValue { get; set; }

        /// <summary>
        /// 建设用地面积
        /// </summary>
        public bool BuildLandAreaValue { get; set; }

        /// <summary>
        /// 未利用地面积
        /// </summary>
        public bool UndueAreaValue { get; set; }

        /// <summary>
        /// 四至索引
        /// </summary>
        public bool LandNeibhborValue { get; set; }

        /// <summary>
        /// 重要界址点点位说明
        /// </summary>
        public bool EmphasisDotCommentValue { get; set; }

        /// <summary>
        /// 主要权属界线走向说明
        /// </summary>
        public bool MainCoilCommentValue { get; set; }

        /// <summary>
        /// 宗地被线状国有或其它农民集体土地分割的说明
        /// </summary>
        public bool DivisionCommentValue { get; set; }

        /// <summary>
        /// 其他说明
        /// </summary>
        public bool OtherCommentValue { get; set; }

        /// <summary>
        /// 土地权属界址调查记事及调查员意见
        /// </summary>
        public bool InvestigateCommentValue { get; set; }

        /// <summary>
        /// 调查人员
        /// </summary>
        public bool InvestigatePersonValue { get; set; }

        /// <summary>
        /// 堪丈日期
        /// </summary>
        public bool InvestigateDateValue { get; set; }

        /// <summary>
        /// 权证编号
        /// </summary>
        public bool WarrantNumberValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public bool CommentValue { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount { get; set; }

        #endregion

        #region Ctor

        public CollectiveLandOutputDefine()
        {
            NameValue = true;
            NatureValue = true;
            DepartmentValue = true;
            LandLocationValue = true;
            RepresentNameValue = true;
            RepresentNumberValue = true;
            RepresentTelphoneValue = true;
            AgentNameValue = true;
            AgentCrdentialNumberValue = true;
            AgentTelphoneValue = true;
            OwnRightTypeValue = true;
            OwnUnitCodeValue = true;
            LandNumberValue = true;
            ImageNumberValue = true;
            AreaValue = true;
            FarmerLandAreaValue = true;
            BuildLandAreaValue = true;
            UndueAreaValue = true;
            LandNeibhborValue = true;
            EmphasisDotCommentValue = true;
            MainCoilCommentValue = true;
            DivisionCommentValue = true;
            OtherCommentValue = true;
            InvestigateCommentValue = true;
            InvestigatePersonValue = true;
            InvestigateDateValue = true;
            WarrantNumberValue = true;
            CommentValue = true;
            ColumnCount = 32;
        }

        #endregion
    }

    public class CollectiveLandOutputDefineCollection //(修改前): YltEntityCollection<CollectiveLandOutputDefine>
    {
    }
}
