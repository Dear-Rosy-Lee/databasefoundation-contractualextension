/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 测绘申请书
    /// </summary>
    public class ExportSurveyBook : AgricultureWordBook
    {
        #region Ctor

        public ExportSurveyBook()
        {
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        ///  承包方
        /// </summary>
        public VirtualPerson VirtualPerson { get; set; }

        #endregion

        #region Methods

        #region Override

        protected override bool OnSetParamValue(object data)
        {
            if (data == null || data.ToString().Length < 1)
                return false;
            base.OnSetParamValue(data);
            SetBookmarkValue("Date", (Date != null && Date.HasValue) ? string.Format("{0: yyyy 年 MM 月 dd 日}", Date) : "    年    月    日");
            return true;
        }

        #endregion

        #region Private

        #endregion

        #endregion
    }
}
