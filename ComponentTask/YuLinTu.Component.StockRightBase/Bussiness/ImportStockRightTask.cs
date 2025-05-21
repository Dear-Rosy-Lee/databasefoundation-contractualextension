/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Component.StockRightBase.Bussiness;
using YuLinTu.Library.Log;
using YuLinTu.Component.StockRightBase.Model;

namespace YuLinTu.Component.StockRightBase.Bussiness
{
    /// <summary>
    /// 导入地块调查表任务类
    /// </summary>
    public class ImportStockRightBaseTask : Task
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportStockRightBaseTask()
        { }

        /// <summary>
        /// 是否确股
        /// </summary>
        public bool IsStockRightBase
        {
            get; set;
        }

        public ReadExcelBase ImportExcelBase { get; set; }

        public BussinessData BussinessData { get; set; }

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportLandTableArgument argument = Argument as TaskImportLandTableArgument;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportWarn("没有找到" + argument.CurrentZone.Name + "对应的调查表");
                this.ReportProgress(100);
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            this.ReportInfomation("开始读取数据");
            this.ReportProgress(1, "开始读取数据");
            ImportExcelBase.DbContext = dbContext;
            ImportExcelBase.CurrentZone = currentZone;
            ImportExcelBase.ExcelName = fileName;
            var readResult = ImportExcelBase.ReadTableInformation();
            if (readResult != null)
            {
                this.ReportError(readResult);
                this.ReportProgress(100,"读取导入数据失败，请检查导入的数据");
                return;
            }
            this.ReportInfomation("数据读取完成，开始导入承包方数据...");
            this.ReportProgress(40, "数据读取完成，开始导入承包方数据...");
            var exportVP=ImportContractors(ImportExcelBase.excelData);//导入承包方数据
            if (!exportVP.IsNullOrEmpty())
            {
                this.ReportError(exportVP);
                this.ReportProgress(100, "导入承包方相关数据失败，请查看信息！");
                return;
            }
            this.ReportInfomation("开始导入地块数据...");
            this.ReportProgress(60, "开始导入地块数据...");
            var exportLands=ImportLandList(ImportExcelBase.excelData);//导入地块数据
            if (!exportVP.IsNullOrEmpty())
            {
                this.ReportError(exportLands);
                this.ReportProgress(100, "导入地块相关数据失败，请查看信息！");
                return;
            }
            this.ReportInfomation("开始导入权属关系...");
            this.ReportProgress(80, "开始导入权属关系...");
            var relationShip=BussinessData.ImportRelationship(ImportExcelBase.excelData);//导入权属关系
            if (!relationShip.IsNullOrEmpty())
            {
                this.ReportError(relationShip);
                this.ReportProgress(100, "导入权属相关数据失败，请查看信息！");
                return;
            }
            this.ReportInfomation("导入数据完成！");
            this.ReportProgress(100, "导入数据完成！");
            ImportExcelBase.LandContractors.Clear();
            ImportExcelBase.Dispose();
            Dispose();
        }



        #region 导入承包方数据

        private string ImportContractors(List<ConvertEntity> ExcelData)
        {
            var contractors = GetContractorList(ExcelData);
            if (contractors.Count != 0)
            {
               return BussinessData.ImprotContractorEntitys(contractors);
            }

            return null;
        }

        /// <summary>
        /// 获取要导入的承包方集合
        /// </summary>
        /// <param name="excelData"></param>
        /// <returns></returns>
        private List<VirtualPerson> GetContractorList(List<ConvertEntity> excelData)
        {
            List<VirtualPerson> vpList = new List<VirtualPerson>();
            var familyNumbers = new List<string>();
            if (excelData != null && excelData.Count > 0)
            {
                foreach (var vp in excelData)
                {
                    if (!familyNumbers.Contains(vp.Contractor.FamilyNumber))
                    {
                        vpList.Add(vp.Contractor);
                    }
                }
            }

            return vpList;
        }

        #endregion

        #region 导入地块数据
        private string ImportLandList(List<ConvertEntity> ExcelData)
        {
            var landList = GetLandList(ExcelData);
            if (landList.Count != 0)
            {
              return  BussinessData.ImportLansEntities(landList);
            }

            return null;
        }



        /// <summary>
        /// 获取去重后的土地
        /// </summary>
        /// <param name="excelData"></param>
        /// <returns></returns>
        private List<ContractLand> GetLandList(List<ConvertEntity> excelData)
        {
            List<ContractLand> landList = new List<ContractLand>();
            List<ContractLand> tempList = new List<ContractLand>();
            List<string> landNumberList = new List<string>();
            if (excelData != null && excelData.Count > 0)
            {
                excelData.ForEach(s =>
                {
                    tempList.AddRange(s.LandList);
                });
            }

            if (tempList.Count > 0)
            {
                tempList.ForEach(s =>
                {
                    if (!landNumberList.Contains(s.LandNumber))
                    {
                        landList.Add(s);
                        landNumberList.Add(s.LandNumber);
                    }
                });
            }

            return landList;
        }

        #endregion
        
    }
}
