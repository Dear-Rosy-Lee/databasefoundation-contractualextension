using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.StockRightBase.Model
{
    /// <summary>
    /// 列索引定义
    /// </summary>
    public static class ColumnDefine
    {
        /// <summary>
        /// 承包方编号(户号)
        /// </summary>
        public const int CONTRACTOR_NUMBER = 0;
        /// <summary>
        /// 承包方姓名
        /// </summary>
        public const int CONTRACTOR_NAME = 1;
        /// <summary>
        /// 承包方类型
        /// </summary>
        public const int CONTRACTOR_TYPE = 2;
        /// <summary>
        /// 家庭成员数
        /// </summary>
        public const int SHARE_PERSON_COUNT = 3;

        #region 家庭成员
        /// <summary>
        /// 姓名
        /// </summary>
        public const int PERSON_NAME = 4;
        /// <summary>
        /// 性别
        /// </summary>
        public const int PERSON_GENDER = 5;
        /// <summary>
        /// 证件类型
        /// </summary>
        public const int PERSON_ID_TYPE = 6;
        /// <summary>
        /// 证件号码
        /// </summary>
        public const int PERSON_ID_NUMBER = 7;
        /// <summary>
        /// 家庭关系
        /// </summary>
        public const int PERSON_RELATION = 8;
        /// <summary>
        /// 是否共有人
        /// </summary>
        public const int PERSON_ISSHARE = 9;
        /// <summary>
        /// 备注
        /// </summary>
        public const int PERSON_COMMENT = 10;
        #endregion

        /// <summary>
        /// 承包方地址
        /// </summary>
        public const int ADDRESS = 11;
        /// <summary>
        /// 邮政编码
        /// </summary>
        public const int POSTAL_CODE = 12;
        /// <summary>
        /// 电话号码
        /// </summary>
        public const int TEL = 13;
        /// <summary>
        /// 承包合同编号
        /// </summary>
        public const int CONCORD_NUMBER = 14;
        /// <summary>
        /// 经营权证编号
        /// </summary>
        public const int WARRANT_NUMBER = 15;
        /// <summary>
        /// 调查员
        /// </summary>
        public const int SURVEY_PERSON = 16;
        /// <summary>
        /// 调查日期
        /// </summary>
        public const int SURVEY_DATE = 17;
        /// <summary>
        /// 调查记事
        /// </summary>
        public const int SURVEY_REMARK = 18;
        /// <summary>
        /// 审核人
        /// </summary>
        public const int CHECK_PERSON = 19;

        #region 地块信息
        /// <summary>
        /// 地块名称
        /// </summary>
        public const int LAND_NAME = 20;
        /// <summary>
        /// 地块编码
        /// </summary>
        public const int LAND_NUMBER = 21;
        /// <summary>
        /// 图幅编号
        /// </summary>
        public const int LAND_PICTURE_NUM = 22;
        /// <summary>
        /// 地块数
        /// </summary>
        public const int LAND_COUNT = 23;
        /// <summary>
        /// 二轮合同面积
        /// </summary>
        public const int LAND_TABLEAREA = 24;
        /// <summary>
        /// 实测面积
        /// </summary>
        public const int LAND_ACTUALAREA = 25;
        /// <summary>
        /// 东至
        /// </summary>
        public const int LAND_EAST = 26;
        /// <summary>
        /// 南至
        /// </summary>
        public const int LAND_SOUTH = 27;
        /// <summary>
        /// 西至
        /// </summary>
        public const int LAND_WEST = 28;
        /// <summary>
        /// 北至
        /// </summary>
        public const int LAND_NORTH = 29;
        /// <summary>
        /// 土地用途
        /// </summary>
        public const int LAND_PURPOSE= 30;
        /// <summary>
        /// 等级
        /// </summary>
        public const int LAND_LEVEL = 31;
        /// <summary>
        /// 土地利用类型
        /// </summary>
        public const int LAND_USEDTYPE = 32;
        /// <summary>
        /// 是否基本农田
        /// </summary>
        public const int LAND_ISFARM = 33;
        /// <summary>
        /// 指界人
        /// </summary>
        public const int LAND_REF = 34;
        /// <summary>
        /// 备注
        /// </summary>
        public const int LAND_COMMENT = 35;
        /// <summary>
        /// 调查员
        /// </summary>
        public const int LAND_SUVPERSON = 36;
        /// <summary>
        /// 调查日期
        /// </summary>
        public const int LAND_SUVDATE = 37;
        /// <summary>
        /// 调查记事
        /// </summary>
        public const int LAND_SUVREMARK = 38;
        /// <summary>
        /// 审核人
        /// </summary>
        public const int LAND_CHECKPERSON = 39;
        /// <summary>
        /// 审核日期
        /// </summary>
        public const int LAND_CHECKDATE = 40;
        /// <summary>
        /// 审核意见
        /// </summary>
        public const int LAND_CHECKOPINION = 41;
        /// <summary>
        /// 共用面积
        /// </summary>
        public const int LAND_SHAREAREA = 42;
        /// <summary>
        /// 合同面积
        /// </summary>
        public const int LAND_CONCORDAREA = 43;
        /// <summary>
        /// 量化户面积
        /// </summary>
        public const int LAND_QUAAREA = 45;
        #endregion
    }
}
