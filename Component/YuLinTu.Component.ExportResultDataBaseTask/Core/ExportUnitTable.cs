/*
 * (C)2015 鱼鳞图公司版权所有，保留所有权利
 */

using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Excel;
using YuLinTu.Library.Entity;
using YuLinTu.Unity;
using YuLinTuQuality.Business.Entity;
using YuLinTuQuality.Business.TaskBasic;
using Zone = YuLinTuQuality.Business.Entity.Zone;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 导出权属单位代码表
    /// </summary>
    public class ExportUnitTable : Task
    {
        #region Fields

        /// <summary>
        /// 表格
        /// </summary>
        private ISheet sheet;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 保存路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// SessionCode
        /// </summary>
        public Guid? SessionCode { private get; set; }

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 发包方集合
        /// </summary>
        public List<CollectivityTissue> Tissues { get; set; }

        /// <summary>
        /// 日志文件
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// 前缀名称
        /// </summary>
        public string PreviewName { get; set; }

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ExportFileEntity ExportFile { get; set; }

        #endregion Propertys

        #region Ctor

        public ExportUnitTable()
        {
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 导出数据
        /// </summary>
        public void ExportData()
        {
            var args = Argument as TaskBuildExportResultDataBaseArgument;
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            if (!ExportFile.UnitCodeTable.IsExport)
                return;
            try
            {
                IProviderExcelFile provider = CreateSheet();
                if (args.ExportLandCode == true)
                {
                    SetZoneCodeSheetCell(provider);
                }
                else
                {
                    SetSheetCell(provider);
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导出权属单位代码表发生错误");
                if (File.Exists(LogFileName))
                {
                    LogWrite.WriteLog(LogFileName, ex.Message + ex.StackTrace);
                }
            }
        }

        private void SetZoneCodeSheetCell(IProviderExcelFile provider)
        {
            if (ZoneList == null || ZoneList.Count == 0 || provider == null)
            {
                return;
            }
            if (Tissues == null || Tissues.Count == 0 || provider == null)
            {
                return;
            }
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            IRow frow = sheet.Rows[0];
            frow.Height = 27;
            ICell cell = frow.Cells[0];
            cell.Value = "权属单位代码";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;

            cell = frow.Cells[1];
            cell.Value = "权属单位名称";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;

            cell = frow.Cells[2];
            cell.Value = "地域编码";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;

            cell = frow.Cells[3];
            cell.Value = "地域名称";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;

            int index = 1;
            foreach (var tissue in Tissues)
            {
                IRow row = sheet.Rows[index];
                string tissueCode = tissue.Code;
                if (tissueCode.Length < 14)
                {
                    tissueCode = tissueCode.PadRight(14, '0');
                }
                if (tissueCode.Length == 16)
                {
                    tissueCode = tissueCode.Substring(0, 12) + tissueCode.Substring(14, 2);
                }
                string tissueName = tissue.Name;
                row.Cells[0].Value = tissueCode;
                row.Cells[1].Value = tissueName;
                index++;
            }
            index = 1;
            foreach (var zone in ZoneList)
            {
                IRow row = sheet.Rows[index];
                string zoneCode = zone.FullCode;
                if (zoneCode.Length < 14)
                {
                    zoneCode = zoneCode.PadRight(14, '0');
                }
                if (zoneCode.Length == 16)
                {
                    zoneCode = zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
                }
                string zoneName = zone.FullName;
                row.Cells[2].Value = zoneCode;
                row.Cells[3].Value = zoneName;
                index++;
            }
            provider.Save();
        }

        /// <summary>
        /// 设置表格值
        /// </summary>
        private void SetSheetCell(IProviderExcelFile provider)
        {
            if (ZoneList == null || ZoneList.Count == 0 || provider == null)
            {
                return;
            }
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            IRow frow = sheet.Rows[0];
            frow.Height = 27;
            ICell cell = frow.Cells[0];
            cell.Value = "权属单位代码";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;

            cell = frow.Cells[1];
            cell.Value = "权属单位名称";
            cell.HorizontalAlignment = eHorizontalAlignment.Center;
            int index = 1;
            foreach (var zone in ZoneList)
            {
                IRow row = sheet.Rows[index];
                string zoneCode = zone.FullCode;
                if (zoneCode.Length < 14)
                {
                    zoneCode = zoneCode.PadRight(14, '0');
                }
                if (zoneCode.Length == 16)
                {
                    zoneCode = zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
                }
                string zoneName = zone.FullName;
                row.Cells[0].Value = zoneCode;
                row.Cells[1].Value = zoneName;
                index++;
            }
            provider.Save();
        }

        /// <summary>
        /// 创建表格
        /// </summary>
        /// <returns></returns>
        private IProviderExcelFile CreateSheet()
        {
            string fileName = FilePath;
            if (!Directory.Exists(fileName))
            {
                return null;
            }
            fileName = Path.Combine(fileName, PreviewName + "权属单位代码表.xls");
            ExcelFileConnectionStringBuilder excelfielcs = new ExcelFileConnectionStringBuilder();
            excelfielcs.FileName = fileName;
            excelfielcs.FileMode = System.IO.FileMode.Create;
            excelfielcs.FileAccess = System.IO.FileAccess.ReadWrite;
            IProviderExcelFile provider = new ProviderExcelFile(excelfielcs.ConnectionString);
            sheet = provider.Sheets.Create("权属单位代码表");
            return provider;
        }

        #endregion Methods
    }
}