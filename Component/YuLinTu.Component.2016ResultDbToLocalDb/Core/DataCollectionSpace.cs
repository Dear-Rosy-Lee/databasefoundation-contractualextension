/*
 * (C) 2016 鱼鳞图公司版权所有,保留所有权利
*/
using Quality.Business.Entity;
using System;
using System.Collections.Generic;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 空间数据集合类
    /// </summary>
    public class DataCollectionSpace
    {
        #region Properties

        /// <summary>
        /// 控制点
        /// </summary>
        public List<KZD> KZD { get; set; }

        /// <summary>
        /// 基本农田保护区
        /// </summary>
        public List<JBNTBHQ> BHQ { get; set; }

        /// <summary>
        /// 区域界线
        /// </summary>
        public List<QYJX> QYJX { get; set; }

        /// <summary>
        /// 点状地物
        /// </summary>
        public List<DZDW> DZDW { get; set; }

        /// <summary>
        /// 线状地物
        /// </summary>
        public List<XZDW> XZDW { get; set; }

        /// <summary>
        /// 面状地物
        /// </summary>
        public List<MZDW> MZDW { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            if (KZD != null)
                KZD.Clear();
            if (BHQ != null)
                BHQ.Clear();
            if (QYJX != null)
                QYJX.Clear();
            if (DZDW != null)
                DZDW.Clear();
            if (XZDW != null)
                XZDW.Clear();
            if (MZDW != null)
                MZDW.Clear();
            GC.Collect();
        }

        #endregion
    }
}
