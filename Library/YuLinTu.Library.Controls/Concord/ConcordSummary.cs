/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 合同统计实体
    /// </summary>
    public class ConcordSummary : NotifyCDObject
    {
        #region Propertys

        private int concordCount;
        private string concordAreaCount;
        private string actualAreaCount;
        private string awareAreaCount;

        #endregion

        #region Propertys

        /// <summary>
        /// 合同数
        /// </summary>
        public int ConcordCount
        {
            get { return concordCount; }
            set
            {
                concordCount = value;
                NotifyPropertyChanged("ConcordCount");
            }
        }

        /// <summary>
        /// 二轮合同总面积
        /// </summary>
        public string ConcordAreaCount
        {
            get { return concordAreaCount; }
            set
            {
                concordAreaCount = value;
                NotifyPropertyChanged("ConcordAreaCount");
            }
        }

        /// <summary>
        /// 实测总面积
        /// </summary>
        public string ActualAreaCount
        {
            get { return actualAreaCount; }
            set
            {
                actualAreaCount = value;
                NotifyPropertyChanged("ActualAreaCount");
            }
        }

        /// <summary>
        /// 确权总面积
        /// </summary>
        public string AwareAreaCount
        {
            get { return awareAreaCount; }
            set
            {
                awareAreaCount = value;
                NotifyPropertyChanged("AwareAreaCount");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 清除统计数据
        /// </summary>
        public void InitialData()
        {
            this.ConcordCount = 0;
            this.ConcordAreaCount = "0.00";
            this.ActualAreaCount = "0.00";
            this.AwareAreaCount = "0.00";
        }

        #endregion
    }
}
