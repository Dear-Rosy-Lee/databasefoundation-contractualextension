/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 台账相关信息统计实体
    /// </summary>
    public class ContractAccountSummary : NotifyCDObject
    {
        #region Fields

        private int familyCount;
        private int landCount;
        private string actualAreaCount = "0.00";
        private string arwareAreaCount = "0.00";
        private string tableAreaCount = "0.00";

        #endregion

        #region Properties

        /// <summary>
        /// 承包方户数
        /// </summary>
        public int FamilyCount
        {
            get { return familyCount; }
            set
            {
                familyCount = value;
                NotifyPropertyChanged("FamilyCount");
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
        /// 总二轮合同面积
        /// </summary>
        public string TableAreaCount
        {
            get { return tableAreaCount; }
            set
            {
                tableAreaCount = value;
                NotifyPropertyChanged("TableAreaCount");
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
        public void EmptyData()
        {
            this.FamilyCount = 0;
            this.LandCount = 0;
            this.ActualAreaCount = "0.00";
            this.ArwareAreaCount = "0.00";
            this.TableAreaCount = "0.00";
        }

        #endregion
    }
}
