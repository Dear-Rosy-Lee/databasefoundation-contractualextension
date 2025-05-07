/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;


namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 农村土地承包经营权集体登记申请书
    /// </summary>
    public class ConcordApplicationPrinter : AgricultureWordBook
    {
        #region Fields
        #endregion

        #region Ctor

        public ConcordApplicationPrinter()
        {
            base.TemplateName = "集体登记申请书";
        }

        #endregion

        #region Methods

        #region Methods - Private

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected  override bool OnSetParamValue(object data)
        {
            ConcordApplicationPrinterData dt = data as ConcordApplicationPrinterData;
            CurrentZone = dt.CurrentZone;
            Tissue = dt.Tissue;        
            base.OnSetParamValue(data);
            SetBookmarkValue("bmYear", dt.Year);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("RequireFamilyCount" + (i == 0 ? "" : i.ToString()), dt.CountContractor);
                SetBookmarkValue("RequireLandCount" + (i == 0 ? "" : i.ToString()), dt.LandCount);
                SetBookmarkValue("RequireLandArea" + (i == 0 ? "" : i.ToString()), dt.AreaFarm);
                SetBookmarkValue("RequireContractMode" + (i == 0 ? "" : i.ToString()), dt.ContractMode);
                SetBookmarkValue("RequireTerm" + (i == 0 ? "" : i.ToString()), dt.ContractTerm);
                SetBookmarkValue("bmContractTerm" + (i == 0 ? "" : i.ToString()), dt.ContractTerm);
                SetBookmarkValue("bmYear" + (i + 1).ToString(), (dt.RequireDate != null && dt.RequireDate.HasValue) ? dt.RequireDate.Value.Year.ToString() : "    ");
                SetBookmarkValue("bmMonth" + (i + 1).ToString(), (dt.RequireDate != null && dt.RequireDate.HasValue) ? dt.RequireDate.Value.Month.ToString() : "    ");
                SetBookmarkValue("bmDay" + (i + 1).ToString(), (dt.RequireDate != null && dt.RequireDate.HasValue) ? dt.RequireDate.Value.Day.ToString() : "    ");
                SetBookmarkValue("bmConcordNumber" + (i == 0 ? "" : i.ToString()), dt.RequireNumber);
                SetBookmarkValue("bmSenderName" + (i == 0 ? "" : i.ToString()), dt.NameTissue);
                SetBookmarkValue("bmArea" + (i == 0 ? "" : i.ToString()), dt.AreaContract);
                SetBookmarkValue("bmLandArea" + (i == 0 ? "" : i.ToString()), dt.AreaFarm);
                SetBookmarkValue("bmOtherLandArea" + (i == 0 ? "" : i.ToString()), dt.AreaOther);
                SetBookmarkValue("bmFamilyNumber" + (i == 0 ? "" : i.ToString()), dt.CountContractor);
                SetBookmarkValue("bmStartTimeYear" + (i == 0 ? "" : i.ToString()), (dt.StartTime != null && dt.StartTime.HasValue) ? dt.StartTime.Value.Year.ToString() : "    ");
                SetBookmarkValue("bmStartTimeMonth" + (i == 0 ? "" : i.ToString()), (dt.StartTime != null && dt.StartTime.HasValue) ? dt.StartTime.Value.Month.ToString() : "    ");
                SetBookmarkValue("bmStartTimeDay" + (i == 0 ? "" : i.ToString()), (dt.StartTime != null && dt.StartTime.HasValue) ? dt.StartTime.Value.Day.ToString() : "    ");
                SetBookmarkValue("bmEndTimeYear" + (i == 0 ? "" : i.ToString()), (dt.EndTime != null && dt.EndTime.HasValue) ? dt.EndTime.Value.Year.ToString() : "    ");
                SetBookmarkValue("bmEndTimeMonth" + (i == 0 ? "" : i.ToString()), (dt.EndTime != null && dt.EndTime.HasValue) ? dt.EndTime.Value.Month.ToString() : "    ");
                SetBookmarkValue("bmEndTimeDay" + (i == 0 ? "" : i.ToString()), (dt.EndTime != null && dt.EndTime.HasValue) ? dt.EndTime.Value.Day.ToString() : "    ");
            }
            WriteZoneExpressBookMark(dt);
            WriteDateExpressInformation();
            base.Destroyed();
            return true;
        }

        /// <summary>
        /// 写地域扩展书签
        /// </summary>
        private void WriteZoneExpressBookMark(ConcordApplicationPrinterData dt)
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("bmNameProvice" + (i == 0 ? "" : i.ToString()), dt.NameProvice);
                SetBookmarkValue("bmSmallProvice" + (i == 0 ? "" : i.ToString()), dt.NameProvice.Substring(0, dt.NameProvice.Length - 1));
                SetBookmarkValue("bmNameCity" + (i == 0 ? "" : i.ToString()), dt.NameCity);
                SetBookmarkValue("bmSmallCity" + (i == 0 ? "" : i.ToString()), dt.NameCity.Substring(0, dt.NameCity.Length - 1));
                SetBookmarkValue("bmCountyName" + (i == 0 ? "" : i.ToString()), dt.NameCounty);
                SetBookmarkValue("bmSmallCounty" + (i == 0 ? "" : i.ToString()), dt.NameCounty.Substring(0, dt.NameCounty.Length - 1));
                SetBookmarkValue("bmNameCounty" + (i == 0 ? "" : i.ToString()), dt.NameCounty);
                SetBookmarkValue("bmNameTown" + (i == 0 ? "" : i.ToString()), dt.NameTown);
                SetBookmarkValue("bmSmallTown" + (i == 0 ? "" : i.ToString()), dt.NameTown.Substring(0, dt.NameTown.Length - 1));
                if (!string.IsNullOrEmpty(dt.NameVillage))
                {
                    SetBookmarkValue("bmVillageNmae" + (i == 0 ? "" : i.ToString()), dt.NameVillage);
                    SetBookmarkValue("bmSmallVillage" + (i == 0 ? "" : i.ToString()), dt.NameVillage.Substring(0, dt.NameVillage.Length - 1));
                }
                if (!string.IsNullOrEmpty(dt.NameGroup))
                {
                    SetBookmarkValue("bmGroupNmae" + (i == 0 ? "" : i.ToString()), dt.NameGroup);
                    SetBookmarkValue("bmSmallGroup" + (i == 0 ? "" : i.ToString()), dt.NameGroup.Substring(0, dt.NameGroup.Length - 1));
                }

                SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), dt.NameProvice);
                SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), dt.NameProvice.Substring(0, dt.NameProvice.Length - 1));
                SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), dt.NameCity);
                SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), dt.NameCity.Substring(0, dt.NameCity.Length - 1));
                SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), dt.NameCounty);
                SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), dt.NameCounty.Substring(0, dt.NameCounty.Length - 1));
                SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), dt.NameTown);
                SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), dt.NameTown.Substring(0, dt.NameTown.Length - 1));
                if (!string.IsNullOrEmpty(dt.NameVillage))
                {
                    SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), dt.NameVillage);
                    SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), dt.NameVillage.Substring(0, dt.NameVillage.Length - 1));
                }
                if (!string.IsNullOrEmpty(dt.NameGroup))
                {
                    SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), dt.NameGroup);
                    SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), dt.NameGroup.Substring(0, dt.NameGroup.Length - 1));
                }
            }
        }

        /// <summary>
        /// 填写日期扩展信息
        /// </summary>
        private void WriteDateExpressInformation()
        {
            for (int i = 0; i < 6; i++)
            {
                string year = DateTime.Now.Year.ToString();
                SetBookmarkValue("NowYear" + (i == 0 ? "" : i.ToString()), year);
                string month = DateTime.Now.Month.ToString();
                SetBookmarkValue("NowMonth" + (i == 0 ? "" : i.ToString()), month);
                string day = DateTime.Now.Day.ToString();
                SetBookmarkValue("NowDay" + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullDate" + (i == 0 ? "" : i.ToString()), fullDate);
                year = ToolMath.GetChineseLowNimeric(DateTime.Now.Year.ToString());
                SetBookmarkValue("ChineseYear" + (i == 0 ? "" : i.ToString()), year);
                month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue("ChineseMonth" + (i == 0 ? "" : i.ToString()), month);
                day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue("ChineseDay" + (i == 0 ? "" : i.ToString()), day);
                fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullChineseDate" + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        #endregion

        #endregion
    }
}
