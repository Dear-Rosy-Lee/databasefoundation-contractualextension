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
    public class CollectiveLandImportDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 所有权人索引
        /// </summary>
        public int NameIndex { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public int NatureIndex { get; set; }

        /// <summary>
        /// 上级主管部门
        /// </summary>
        public int DepartmentIndex { get; set; }

        /// <summary>
        /// 土地座落
        /// </summary>
        public int LandLocationIndex { get; set; }

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
        /// 权属单位代码
        /// </summary>
        public int OwnUnitCodeIndex { get; set; }

        /// <summary>
        /// 宗地号
        /// </summary>
        public int LandNumberIndex { get; set; }

        /// <summary>
        /// 图号
        /// </summary>
        public int ImageNumberIndex { get; set; }

        /// <summary>
        /// 总面积
        /// </summary>
        public int AreaIndex { get; set; }

        /// <summary>
        /// 农用地面积
        /// </summary>
        public int FarmerLandAreaIndex { get; set; }

        /// <summary>
        /// 建设用地面积
        /// </summary>
        public int BuildLandAreaIndex { get; set; }

        /// <summary>
        /// 机动地总面积索引
        /// </summary>
        public int TotalMotorizeAreaIndex { get; set; }

        /// <summary>
        /// 未利用地面积
        /// </summary>
        public int UndueAreaIndex { get; set; }

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
        /// 重要界址点点位说明
        /// </summary>
        public int EmphasisDotCommentIndex { get; set; }

        /// <summary>
        /// 主要权属界线走向说明
        /// </summary>
        public int MainCoilCommentIndex { get; set; }

        /// <summary>
        /// 宗地被线状国有或其它农民集体土地分割的说明
        /// </summary>
        public int DivisionCommentIndex { get; set; }

        /// <summary>
        /// 其他说明
        /// </summary>
        public int OtherCommentIndex { get; set; }

        /// <summary>
        /// 土地权属界址调查记事及调查员意见
        /// </summary>
        public int InvestigateCommentIndex { get; set; }

        /// <summary>
        /// 调查人员
        /// </summary>
        public int InvestigatePersonIndex { get; set; }

        /// <summary>
        /// 堪丈日期
        /// </summary>
        public int InvestigateDateIndex { get; set; }

        /// <summary>
        /// 权证编号
        /// </summary>
        public int WarrantNumberIndex { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public int CommentIndex { get; set; }

        #endregion

        #region Ctor

        public CollectiveLandImportDefine()
        {
            NameIndex = 1;
            NatureIndex = 2;
            DepartmentIndex = 3;
            LandLocationIndex = 4;
            RepresentNameIndex = 5;
            RepresentNumberIndex = 6;
            RepresentTelphoneIndex = 7;
            AgentNameIndex = 8;
            AgentCrdentialNumberIndex = 9;
            AgentTelphoneIndex = 10;
            OwnRightTypeIndex = 11;
            OwnUnitCodeIndex = 12;
            LandNumberIndex = 13;
            ImageNumberIndex = 14;
            AreaIndex = 15;
            FarmerLandAreaIndex = 16;
            BuildLandAreaIndex = 17;
            UndueAreaIndex = 18;
            EastIndex = 19;
            SourthIndex = 20;
            WestIndex = 21;
            NorthIndex = 22;
            EmphasisDotCommentIndex = 23;
            MainCoilCommentIndex = 24;
            DivisionCommentIndex = 25;
            OtherCommentIndex = 26;
            InvestigateCommentIndex = 27;
            InvestigatePersonIndex = 28;
            InvestigateDateIndex = 29;
            WarrantNumberIndex = 30;
            CommentIndex = 31;
        }

        #endregion
    }

    public class CollectiveLandImportDefineCollection //(修改前): YltEntityCollection<CollectiveLandImportDefine>
    {
    }
}
