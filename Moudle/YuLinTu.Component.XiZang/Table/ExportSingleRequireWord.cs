/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 预览单户申请表
    /// </summary>
    public class ExportSingleRequireWord : ExportLandRequireBook
    {
        #region Field

        private YuLinTu.Library.Entity.VirtualPerson curFamily;  //当前承包方

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportSingleRequireWord(YuLinTu.Library.Entity.VirtualPerson family) : base(family)
        {
            curFamily = family;
        }

        #endregion

        #region Method

        /// <summary>
        /// 设置文本值
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            bool result = base.OnSetParamValue(data);
            VirtualPerson vp = data as VirtualPerson;
            if (vp == null)
            {
                return false;
            }
            var groupName = string.Empty;
            var dbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = dbContext.CreateZoneWorkStation();
            var concordStation = dbContext.CreateConcordStation();
            var concords = concordStation.GetByZoneCode(vp.ZoneCode);
            var concord = concords.Find(s=>s.ContracterId==vp.ID);
            if (vp.ZoneCode.Length >= Zone.ZONE_GROUP_LENGTH)
            {
                Zone group = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_GROUP_LENGTH));
                groupName = group != null ? group.Name : "";
            }
            SetBookmarkValue("ContractorOtherCardNumber", curFamily.Number);
            SetBookmarkValue("Group", groupName);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("bmFamilyName" + (i == 0 ? "" : i.ToString()), curFamily.Name);
            }
            
            SetBookmarkValue("ConcordActualAreaCount", concord==null?"":concord.CountActualArea.ToString("0.00"));//实测面积
            SetBookmarkValue("bmLandArea", concord == null ? "" : concord.CountAwareArea.ToString("0.00"));//合同面积
            WriteStartAndEnd(concord);
            return result;
        }

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd(ContractConcord concord)
        {
            if (concord != null)
            {
                string startYear = !concord.ArableLandStartTime.HasValue ? "" : concord.ArableLandStartTime.Value.Year.ToString();
                string startMonth = !concord.ArableLandStartTime.HasValue ? "" : concord.ArableLandStartTime.Value.Month.ToString();
                string startDay = !concord.ArableLandStartTime.HasValue ? "" : concord.ArableLandStartTime.Value.Day.ToString();
                string endYear = !concord.ArableLandEndTime.HasValue ? "" : concord.ArableLandEndTime.Value.Year.ToString();
                string endMonth = !concord.ArableLandEndTime.HasValue ? "" : concord.ArableLandEndTime.Value.Month.ToString();
                string endDay = !concord.ArableLandEndTime.HasValue ? "" : concord.ArableLandEndTime.Value.Day.ToString();

                SetBookmarkValue("ConcordStartYearDate", startYear);
                SetBookmarkValue("ConcordStartMonthDate", startMonth);
                SetBookmarkValue("ConcordStartDayDate", startDay);
                SetBookmarkValue("ConcordEndYearDate", !concord.Flag? endYear:"9999");
                SetBookmarkValue("ConcordEndMonthDate", !concord.Flag ? endMonth:"1");
                SetBookmarkValue("ConcordEndDayDate", !concord.Flag ? endDay:"1");
            }

        }

        #endregion
    }
}
