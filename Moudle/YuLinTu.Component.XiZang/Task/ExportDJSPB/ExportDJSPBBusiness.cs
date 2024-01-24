/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    public class ExportDJSPBBusiness : Task
    {
              
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportDJSPBBusiness()
        {
      
        }

        #endregion

        /// <summary>
        /// 预览登记审批表
        /// </summary>
        public bool PrintRequreBook(List<ContractLand> lands, int SendersCount, List<YuLinTu.Library.Entity.VirtualPerson> VirtualPersons)
        {
            try
            {                
                string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记审批表");
                ExportDJSPBTable printBook = new ExportDJSPBTable();
                printBook.SendersCount = SendersCount;
                printBook.ConstructType = ((int)eConstructMode.Family).ToString();               
                printBook.VirtualPersons = VirtualPersons;            
                printBook.OpenTemplate(tempPath);
                printBook.PrintPreview(lands);
                lands = null;
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 导出登记审批表
        /// </summary>
        public bool SingleExportRequireBook(Zone exportzone, List<ContractLand> lands)
        {
            try
            {
                TaskExportDJSPBArgument metadata = Argument as TaskExportDJSPBArgument;
                string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记审批表");
                string exportfilename = GetZoneName(exportzone);
                ExportDJSPBTable printBook = new ExportDJSPBTable();
                printBook.CurrentZone = metadata.CurrentZone;              
                printBook.ConstructType = ((int)eConstructMode.Family).ToString();
                printBook.SendersCount = metadata.SendersCount;                
                printBook.VirtualPersons = metadata.VirtualPersons;
                printBook.OpenTemplate(tempPath);
                string filePath = metadata.FileName + @"\" + exportfilename + "农村土地承包经营权登记审批表";
                printBook.SaveAs(lands, filePath);
                this.ReportInfomation(string.Format("成功导出{0}登记审批表", exportfilename));
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        public string GetZoneName(Zone zone)
        {
            string name = zone.Name;
            TaskExportDJSPBArgument metadata = Argument as TaskExportDJSPBArgument;
            var db = metadata.Database;
            var zoneStation = db.CreateZoneWorkStation();
            try
            {
                Zone tempZone = zoneStation.Get(zone.UpLevelCode);
                while (tempZone.Level <= eZoneLevel.Village)
                {
                    name = tempZone.Name + name;
                    tempZone = zoneStation.Get(tempZone.UpLevelCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetZoneName(获取地域名称)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域名称出错," + ex.Message);
            }
            return name;
        }



    }
}
