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
    /// 承包方统计实体
    /// </summary>
    public class VirtualPersonSummary : NotifyCDObject
    {
        #region Propertys

        private int familyCount;
        private int personCount;
        private int maleCount;
        private int feMaleCount;
        private int unknowGenderCount;

        #endregion

        #region Propertys

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
        /// 所有人数
        /// </summary>
        public int PersonCount
        {
            get { return personCount; }
            set
            {
                personCount = value;
                NotifyPropertyChanged("PersonCount");
            }
        }

        /// <summary>
        /// 总男性人数
        /// </summary>
        public int MaleCount
        {
            get { return maleCount; }
            set
            {
                maleCount = value;
                NotifyPropertyChanged("MaleCount");
            }
        }

        /// <summary>
        /// 总女性人数
        /// </summary>
        public int FeMaleCount
        {
            get { return feMaleCount; }
            set
            {
                feMaleCount = value;
                NotifyPropertyChanged("FeMaleCount");
            }
        }

        /// <summary>
        /// 未知性别人数
        /// </summary>
        public int UnknowGenderCount
        {
            get { return unknowGenderCount; }
            set
            {
                unknowGenderCount = value;
                NotifyPropertyChanged("UnknowGenderCount");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 清除统计数据
        /// </summary>
        public void InitialData()
        {
            this.FamilyCount = 0;
            this.FeMaleCount = 0;
            this.MaleCount = 0;
            this.PersonCount = 0;
            this.UnknowGenderCount = 0;
        }

        #endregion
    }
}
