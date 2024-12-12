/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    #region Properties

    /// <summary>
    /// 查找四至配置
    /// </summary>
    public class SeekLandNeighborSetting
    {
        /// <summary>
        /// 单方向单名称查找
        /// </summary>
        public bool SimplePositionQuery { set; get; }

        /// <summary>
        /// 某方向为空时使用村民小组名称填充
        /// </summary>
        public bool UseGroupName { set; get; }

        /// <summary>
        /// 某方向为空时使用村民小组名称填充
        /// </summary>
        public string UseGroupNameContext { set; get; }

        /// <summary>
        /// 查找地块名称
        /// </summary>
        public bool SearchLandName { set; get; }

        /// <summary>
        /// 识别地块类别
        /// </summary>
        public bool LandIdentify { set; get; }

        /// <summary>
        /// 识别土地利用类型
        /// </summary>
        public bool LandType { set; get; }

        /// <summary>
        /// 只填充空白四至
        /// </summary>
        public bool FillEmptyNeighbor { set; get; }

        /// <summary>
        /// 查询线状\面状地物
        /// </summary>
        public bool IsQueryXMzdw { set; get; }

        /// <summary>
        /// 同至去重复地物名称
        /// </summary>
        public bool IsDeleteSameDWMC { set; get; }

        /// <summary>
        /// 查找规则   "默认" 为0, "地块优先" 1,"距离优先" 2
        /// </summary>
        public int SearchDeteilRule { set; get; }

        /// <summary>
        /// 设置宗地查找缓冲距离(米)
        /// </summary>
        public double BufferDistance { set; get; }

        /// <summary>
        /// 查询阈值(米)
        /// </summary>
        public double QueryThreshold { set; get; }

        public bool OnlyCurrentZone { get; set; }

    }

    #endregion
}
