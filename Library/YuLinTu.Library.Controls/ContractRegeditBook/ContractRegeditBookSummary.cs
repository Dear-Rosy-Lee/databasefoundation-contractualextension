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
    /// 承包权证相关信息统计实体
    /// </summary>
    public class ContractRegeditBookSummary : NotifyCDObject
    {
        #region Fields

        private int warrantCount;
        private int landCount;
        private string actualAreaCount;
        private string arwareAreaCount;

        #endregion

        #region Properties

        /// <summary>
        /// 权证总数
        /// </summary>
        public int WarrantCount
        {
            get { return warrantCount; }
            set
            {
                warrantCount = value;
                NotifyPropertyChanged("WarrantCount");
            }
        }

        /// <summary>
        /// 总地块数
        /// </summary>
        public int LandCount
        {
            get { return landCount; }
            set
            {
                landCount = value;
                NotifyPropertyChanged("LandCount");
            }
        }

        /// <summary>
        /// 总实测面积
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
        /// 总确权面积
        /// </summary>
        public string ArwareAreaCount
        {
            get { return arwareAreaCount; }
            set
            {
                arwareAreaCount = value;
                NotifyPropertyChanged("ArwareAreaCount");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 清空数据库
        /// </summary>
        public void InitialData()
        {
            this.WarrantCount = 0;
            this.LandCount = 0;
            this.ActualAreaCount = "0.00";
            this.ArwareAreaCount = "0.00";
        }

        #endregion
    }
}
