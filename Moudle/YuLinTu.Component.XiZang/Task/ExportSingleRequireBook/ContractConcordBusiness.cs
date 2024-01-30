/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 此类用于处理承包合同底层业务
    /// </summary>
    public class ContractConcordBusiness : Task
    {
        #region Ctor

        public ContractConcordBusiness()
        { }

        #endregion

        #region Field

        private string messageWarn;

        #endregion

        #region Property

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 预览/导出单户申请书
        /// </summary>
        public bool PrintRequireBookWord(Zone zone, YuLinTu.Library.Entity.VirtualPerson vp, IDbContext dbContext, string fileName, out string message)
        {
            if (vp == null || zone == null)
            {
                message = "地域为空!" + "\n";
                return false;
            }
            bool flag = false;
            string tempPath = "西藏农村土地承包经营权单户登记申请书";
            string concordWarnInfo = string.Format("当前承包方{0}没有签订家庭承包方式合同!", vp.Name);
            string landWarnInfo = string.Format("当前承包方{0}没有家庭承包方式的地块!", vp.Name);
            flag = PrintRequireBook(zone, vp, tempPath, dbContext, concordWarnInfo, landWarnInfo, eConstructMode.Family, fileName);
            message = messageWarn;
            return flag;
        }

        /// <summary>
        /// 预览申请书
        /// </summary>
        private bool PrintRequireBook(Zone zone, YuLinTu.Library.Entity.VirtualPerson vp, string templemPath, IDbContext dbContext,
                                     string warnNoConcordInfo, string warnNoLandInfo, eConstructMode typeMode, string fileName)
        {
            try
            {
                if (vp == null || zone == null)
                {
                    messageWarn = "地域为空!";
                    return false;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var landStation = dbContext.CreateContractLandWorkstation();

                var concords = concordStation.GetAllConcordByFamilyID(vp.ID);
                var zoneList = zoneStation.GetParentsToProvince(zone);
                zoneList.Add(zone);
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                ExportSingleRequireWord printBook = new ExportSingleRequireWord(vp);
                printBook.CurrentZone = zone;
                printBook.ConstructMode = typeMode;
                printBook.ZoneList = zoneList;
                printBook.RequireDate = PublishDateSetting.PublishStartDate;
                printBook.CheckDate = PublishDateSetting.PublishEndDate;
                printBook.DictList = DictList;
                printBook.Concords = concords;
                if (printBook.Concords == null || printBook.Concords.Count == 0)
                {
                    messageWarn = string.Format("{0}下未发现对应签订的合同", vp.Name);
                    return false;
                }
                ContractConcord nowconrd = new ContractConcord();
                var concord = printBook.InitalizeConcord(nowconrd);
                if (concord == null)
                {
                    messageWarn = warnNoConcordInfo;
                    return false;
                }
                printBook.ConcordLands = landStation.GetByConcordId(concord.ID);
                if (printBook.ConcordLands == null || printBook.ConcordLands.Count == 0)
                {
                    messageWarn = string.Format("未获取到合同对应地块集合,合同ID为{0}", concord.ID.ToString());
                    return false;
                }
                printBook.Lands = landStation.GetCollection(vp.ID);
                if (printBook.Lands == null || printBook.Lands.Count == 0)
                {
                    messageWarn = string.Format("未获取到承包方对应地块集合,名称:{0}", vp.Name.ToString());
                    return false;
                }
                List<ContractLand> landuse = new List<ContractLand>();
                if (typeMode != eConstructMode.Family)
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode != ((int)eConstructMode.Family).ToString());
                }
                else
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode == ((int)eConstructMode.Family).ToString());
                }

                if (landuse.Count == 0)
                {
                    messageWarn = warnNoLandInfo;
                    this.ReportWarn(warnNoLandInfo);
                    return false;
                }
                printBook.OpenTemplate(tempPath);
                if (string.IsNullOrEmpty(fileName))
                {
                    printBook.PrintPreview(vp);
                }
                else
                {
                    string filePath = fileName + @"\" + vp.FamilyNumber.PadLeft(4, '0') + "-" + vp.Name + "-" + templemPath;
                    printBook.SaveAs(vp, filePath);
                }
                vp = null;
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "PrintRequireBook(预览/导出申请书)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        #endregion
    }
}
