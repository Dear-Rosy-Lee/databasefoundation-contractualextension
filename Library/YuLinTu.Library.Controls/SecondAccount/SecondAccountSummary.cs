/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 二轮台账统计信息绑定实体
    /// </summary>
    public class SecondAccountSummary : NotifyCDObject
    {
        #region Fields

        /// <summary>
        /// 二轮台账承包方数量
        /// </summary>
        private int virtualPersonCount;

        /// <summary>
        /// 二轮台账地块数量
        /// </summary>
        private int secondLandCount;

        /// <summary>
        /// 二轮总台账面积
        /// </summary>
        private double totalTableArea;

        #endregion

        #region Properties

        /// <summary>
        /// 二轮台账承包方数量
        /// </summary>
        public int VirtualPersonCount
        {
            get { return virtualPersonCount; }
            set
            {
                virtualPersonCount = value;
                NotifyPropertyChanged("VirtualPersonCount");
            }
        }

        /// <summary>
        /// 二轮台账地块数量
        /// </summary>
        public int SecondLandCount
        {
            get { return secondLandCount; }
            set
            {
                secondLandCount = value;
                NotifyPropertyChanged("SecondLandCount");
            }
        }

        /// <summary>
        /// 二轮总台账面积
        /// </summary>
        public double TotalTableArea
        {
            get { return totalTableArea; }
            set
            {
                totalTableArea = value;
                NotifyPropertyChanged("TotalTableArea");
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// 清除统计数据
        /// </summary>
        public void InitialData()
        {
            this.VirtualPersonCount = 0;
            this.SecondLandCount = 0;
            this.TotalTableArea = 0;
        }

        #endregion
    }
}
