/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;


namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包合同集体申请表设置
    /// </summary>
    public class ConcordApplicationSet : NotifyCDObject
    {
        #region Fields

        private DateTime? checkDate;
        private string yearNumber;
        private string applicationBookNumber;

        #endregion

        #region Properties

        /// <summary>
        /// 公示开始日期
        /// </summary>
        public DateTime? CheckDate
        {
            get { return checkDate; }
            set { checkDate = value; NotifyPropertyChanged("CheckDate"); }
        }

        /// <summary>
        /// 年号
        /// </summary>
        public string YearNumber
        {
            get { return yearNumber; }
            set { yearNumber = value; NotifyPropertyChanged("YearNumber"); }
        }

        /// <summary>
        /// 申请书编号
        /// </summary>
        public string ApplicationBookNumber
        {
            get { return applicationBookNumber; }
            set { applicationBookNumber = value; NotifyPropertyChanged("ApplicationBookNumber"); }
        }

        #endregion

        #region Ctor

        public ConcordApplicationSet()
        {
            CheckDate = null;
            YearNumber = "";
            ApplicationBookNumber = "";
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ConcordApplicationSet GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ConcordApplicationSet>();
            var section = profile.GetSection<ConcordApplicationSet>();
            return section.Settings;
        }

        #endregion
    }
}
