using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 重置流水号任务
    /// </summary>
    public class TaskResetAllSerialNumberOperation : Task
    {
        public TaskResetAllSerialNumberOperation()
        {
        }

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportRegeditBookArgument argument = Argument as TaskExportRegeditBookArgument;
            if (argument == null)
            {
                return;
            }
            var dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            var isStock = argument.IsStockRight;
            var config = ContractRegeditBookSettingDefine.GetIntence();
            try
            {
                if (zone == null)
                    return;
                int serialNumber = config.MinNumber;
                var zoneStation = dbContext.CreateZoneWorkStation();
                //var parentsToProvince = zoneStation.GetParentsToProvince(zone);
                //var county = parentsToProvince.Find(c => c.Level == eZoneLevel.County) ?? zone;
                var bookBusiness = new ContractRegeditBookBusiness(dbContext);
                var stockBookStation = dbContext.CreateStockWarrantWorkStation();

                var currentCountyBooks = stockBookStation.Count(zone.FullCode, eLevelOption.SelfAndSubs);
                currentCountyBooks += bookBusiness.Count(zone.FullCode, eLevelOption.SelfAndSubs);

                if (currentCountyBooks == 0)
                    return;

                var countySubZones = zoneStation.GetChildren(zone.FullCode, eLevelOption.SelfAndSubs);//获取镇下的所有地域
                //InsertTop(countySubZones, zone);//将当前地域插入到要排序集合的第一位
                //var resetList = countySubZones.FindAll(s => s.Level == zone.Level);

                int index = 0;
                foreach (var item in countySubZones)
                {
                    var books = bookBusiness.GetByZoneCode(item.FullCode, eSearchOption.Precision);
                    var stockBooks = stockBookStation.GetByZoneCode(item.FullCode, eLevelOption.Self);

                    dbContext.BeginTransaction();
                    books = books.OrderBy(t => t.RegeditNumber).ToList();
                    foreach (var book in books)
                    {
                        book.SerialNumber = (serialNumber++).ToString();
                        bookBusiness.Update(book);

                        int percent = (int)(((double)++index) / currentCountyBooks * 100);
                        this.ReportProgress(percent, string.Format("重置{0}的流水号", item.Name));
                    }
                    stockBooks = stockBooks.OrderBy(t => t.RegeditNumber).ToList();
                    foreach (var book in stockBooks)
                    {
                        book.SerialNumber = (serialNumber++).ToString();
                        stockBookStation.Update(book);

                        int percent = (int)(((double)++index) / currentCountyBooks * 100);
                        this.ReportProgress(percent, string.Format("重置{0}的流水号", item.Name));
                    }
                    dbContext.CommitTransaction();

                    if (item.FullCode == zone.FullCode)
                    {
                        serialNumber = config.MaxNumber + 1 >= serialNumber ? config.MaxNumber + 1 : serialNumber;
                    }

                    if (books.Count > 0 || stockBooks.Count > 0)
                        this.ReportInfomation(string.Format("已经重置了{0}的{1}条流水号", item.Name, books.Count + stockBooks.Count));
                }
            }
            catch (Exception ex)
            {
                this.ReportException(ex);
            }
        }

        #endregion Method—Override

        /// <summary>
        /// 将元素插入到集合的顶部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcList"></param>
        /// <param name="insertObj"></param>
        private void InsertTop(List<Zone> srcList, Zone insertObj)
        {
            if (srcList != null && srcList.Count != 0)
            {
                var objIndex = srcList.FindIndex(s => s.FullCode == insertObj.FullCode);
                if (objIndex != -1)
                {
                    for (int i = objIndex; i > 0; i--)
                    {
                        srcList[i] = srcList[i - 1];//将集合中的元素依次后移
                    }

                    srcList[0] = insertObj;
                }
            }
        }
    }
}