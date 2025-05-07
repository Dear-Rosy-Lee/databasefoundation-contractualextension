/*
 * (C)2015 鱼鳞图公司版权所有，保留所有权利
 */
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YuLinTu.Data;
using YuLinTu.Excel;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 数据统计处理类
    /// </summary>
    public class ExportSummaryTable
    {
        #region Fields

        private List<DataSummary> sumList;
        private string filePath;
        private string codeName;

        #endregion

        #region Property

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ExportFileEntity ExportFile { get; set; }
               
        #endregion

        #region Ctor

        public ExportSummaryTable(List<DataSummary> sumList, string path, string codeName)
        {
            this.sumList = sumList;
            if (sumList == null)
            {
                sumList = new List<DataSummary>();
            }
            this.filePath = path;
            this.codeName = codeName;           
            InitialSummary();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 统计数据
        /// </summary>
        public static void SummaryData(DataCollection data, DataSummary summary,
            CbdkxxAwareAreaExportEnum cBDKXXAwareAreaExportSet, List<SqliteDK> sqliteLand)
        {
            string cbd = ((int)eDKLB.CBD).ToString();
            string zld = ((int)eDKLB.ZLD).ToString();
            string jdd = ((int)eDKLB.JDD).ToString();
            string khd = ((int)eDKLB.KHD).ToString();
            string qtd = ((int)eDKLB.OTHER).ToString();
            string yes = ((int)eSF.Yes).ToString();
            string no = ((int)eSF.No).ToString();
            string qtyt = ((int)eTDYT.OTHER).ToString();
            string zzy = ((int)eTDYT.ZZY).ToString();//种植业
            string ly = ((int)eTDYT.LY).ToString();
            string xmy = ((int)eTDYT.XMY).ToString();
            string yy = ((int)eTDYT.YY).ToString();//渔业
            string gyxz = ((int)eSYQXZ.GYTDSYQ).ToString();
            string cjzz = ((int)eSYQXZ.CJJTJJZZ).ToString();
            string cmxz = ((int)eSYQXZ.CMXZ).ToString();
            string jtsy = ((int)eSYQXZ.JTTDSYQ).ToString();
            string qtsy = ((int)eSYQXZ.Other).ToString();
            string xjsy = ((int)eSYQXZ.XJJTJJZZ).ToString();
            string family = ((int)eCBFLX.Family).ToString();
            string personal = ((int)eCBFLX.Personal).ToString();
            string unit = ((int)eCBFLX.Unit).ToString();

            var areaDic = new Dictionary<string, double>();
            data.CBDKXXJH.ForEach(t =>
            {
                if (!areaDic.ContainsKey(t.DKBM))
                    areaDic.Add(t.DKBM, t.HTMJ);
                else
                    areaDic[t.DKBM] = areaDic[t.DKBM] + t.HTMJ;
            });

            summary.SenderCount = data.FBFJH.Count;
            summary.RegisterBookNumber = data.CBJYQZJH.Count;

            var kjdk = data.KJDKJH;
            kjdk.AddRange(sqliteLand);
            List<SqliteDK> cbdList = kjdk.FindAll(t => t.DKLB == cbd);//承包地
            List<SqliteDK> fcbdList = kjdk.FindAll(t => t.DKLB != cbd);//非承包地
            //从签订合同的地块中选出承包地块
            var cbddkbmlist = cbdList.Select(cbddkbm => cbddkbm.DKBM).ToList();          
            var mdbcbdlistDic = areaDic.Where(dd => cbddkbmlist.Contains(dd.Key)).ToList();

            summary.ContractLandCount = mdbcbdlistDic.Count;//承包地块数
            summary.ContractLandAreaCount = ToolMath.ConvertRound(mdbcbdlistDic.Sum(t => t.Value.ConvertHectare()), 2);// AreaQQSum(cbdList, areaDic);// cbdList.Sum(t => t.SCMJ).ConvertHectare();//承包地块总面积

            //选出有空间信息的非承包地块
            var fcbdshapeList = fcbdList.FindAll(fc => fc.Shape != null);
            summary.UnContractLandCount = fcbdshapeList.Count;//非承包地块数
            summary.UnContractLandAreaCount = AreaSCSum(fcbdshapeList);// fcbdList.Sum(t => t.SCMJ).ConvertHectare();//非承包地块总面积
            summary.UnContractLandTypeArea = summary.UnContractLandAreaCount;
            List<SqliteDK> nyytList = cbdList.FindAll(t => t.TDYT == zzy || t.TDYT == ly || t.TDYT == xmy || t.TDYT == yy);//农业用途地块
            List<SqliteDK> fnyytList = cbdList.FindAll(t => t.TDYT == qtyt);//非农业用途地块
            //summary.AgricureAndUnAreaCount = summary.ContractLandAreaCount + summary.UnContractLandAreaCount; //cbdList.Sum(t => t.SCMJ).ConvertHectare();// summary.ContractLandAreaCount + summary.UnContractLandAreaCount;

            summary.PlantAreaCount = AreaQQSum(nyytList.Where(t => t.TDYT == zzy), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//种植业面积
            summary.ForestAreaCount = AreaQQSum(nyytList.Where(t => t.TDYT == ly), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//林业面积
            summary.AnimalAreaCount = AreaQQSum(nyytList.Where(t => t.TDYT == xmy), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//畜牧业面积
            summary.FishAreaCount = AreaQQSum(nyytList.Where(t => t.TDYT == yy), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//渔业面积

            summary.UnAgricureAreaCount = AreaSCSum(fnyytList);//.Sum(t => t.SCMJ).ConvertHectare();
            summary.AgricureAreaCount = (summary.PlantAreaCount + summary.ForestAreaCount + summary.AnimalAreaCount + summary.FishAreaCount);
            summary.AgricureAndUnAreaCount = summary.AgricureAreaCount + summary.UnAgricureAreaCount;
            List<SqliteDK> gyList = cbdList.FindAll(t => t.SYQXZ == gyxz);//国有地块
            List<SqliteDK> jtList = cbdList.FindAll(t => t.SYQXZ == cjzz || t.SYQXZ == cmxz ||
                t.SYQXZ == jtsy || t.SYQXZ == qtsy || t.SYQXZ == xjsy);//集体地块
            summary.CountryAreaCount = AreaQQSum(gyList, areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//国有面积
            summary.CollectivityArea = AreaQQSum(jtList, areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//集体面积
            summary.CountryAndCollAreaCount = summary.CountryAreaCount + summary.CollectivityArea;//所有面积
            summary.GroupAreaCount = AreaQQSum(jtList.Where(t => t.SYQXZ == cmxz), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//村民小组面积
            summary.GroupCollectivityArea = AreaQQSum(jtList.Where(t => t.SYQXZ == cjzz), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//村级集体经济组织面积
            summary.TownCollectivityArea = AreaQQSum(jtList.Where(t => t.SYQXZ == xjsy), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//乡级集体经济组织面积
            summary.OtherCollectivityArea = AreaQQSum(jtList.Where(t => t.SYQXZ == qtsy), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//其他集体经济组织面积

            summary.SelfLandAreaCount = AreaSCSum(fcbdshapeList.Where(t => t.DKLB == zld));//.Sum(t => t.SCMJ).ConvertHectare();//自留地面积
            summary.MoveLandAreaCount = AreaSCSum(fcbdshapeList.Where(t => t.DKLB == jdd));//.Sum(t => t.SCMJ).ConvertHectare();//机动地面积
            summary.WastLandAreaCount = AreaSCSum(fcbdshapeList.Where(t => t.DKLB == khd));//.Sum(t => t.SCMJ).ConvertHectare();//开荒地
            summary.OtherCollectivityLandArea = AreaSCSum(fcbdshapeList.Where(t => t.DKLB == qtd));//.Sum(t => t.SCMJ).ConvertHectare();//其他集体土地
           
            if (cBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.确权面积)
            {
                summary.ContractIsBaseArea = AreaQQSum(cbdList.Where(t => t.SFJBNT == yes), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//基本农田面积          
                summary.ContractNotBaseArea = AreaQQSum(cbdList.Where(t => t.SFJBNT == no), areaDic);//.Sum(t => t.SCMJ).ConvertHectare();//基本农田面积
            }
            else if (cBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.实测面积)
            {
                summary.ContractIsBaseArea = AreaSCSum(cbdList.Where(t => t.SFJBNT == yes));//.Sum(t => t.SCMJ).ConvertHectare();//基本农田面积          
                summary.ContractNotBaseArea = AreaSCSum(cbdList.Where(t => t.SFJBNT == no));//.Sum(t => t.SCMJ).ConvertHectare();//非基本农田面积
            }
            summary.ContractBaseAreaCount = summary.ContractIsBaseArea + summary.ContractNotBaseArea;//承包方是否基本农田合计

            summary.FamilyContractBookNumber = data.CBJYQZJH.Count(t => !string.IsNullOrEmpty(t.CBJYQZBM) && t.CBJYQZBM.Substring(t.CBJYQZBM.Length - 1) == "J");//家庭承包权证数
            summary.OtherContractBookNumber = data.CBJYQZJH.Count(t => !string.IsNullOrEmpty(t.CBJYQZBM) && t.CBJYQZBM.Substring(t.CBJYQZBM.Length - 1) == "Q");//其他承包权证数

            summary.GisterbookArea = AreaHTSum(data.HTJH, data.CBJYQZJH);//.Sum(t => t.HTZMJ);//权证总面积
            summary.FamilyCount = data.CBFJH.Count;
            List<CBF> jtcbList = data.CBFJH.FindAll(t => t.CBFLX == family);//家庭承包
            List<CBF> grList = data.CBFJH.FindAll(t => t.CBFLX == personal);//个人承包
            List<CBF> dwList = data.CBFJH.FindAll(t => t.CBFLX == unit);//单位承包
            summary.PersonalFamilyCount = grList.Count;
            summary.UnitFamilyCount = dwList.Count;
            summary.ContractFamilyCount = jtcbList.Count;
            summary.OtherFamilyCount = summary.PersonalFamilyCount + summary.UnitFamilyCount;
            summary.ContractFamilyPersonCount = jtcbList.Sum(t => t.CBFCYSL);
        }

        /// <summary>
        /// 统计实测亩面积
        /// </summary>
        static public double AreaHTSum(List<CBHT> hts, List<CBJYQZ> qzs)
        {
            if (qzs == null || qzs.Count == 0)
                return 0;
            HashSet<string> codeColl = new HashSet<string>();
            qzs.ForEach(t => codeColl.Add(t.CBJYQZBM));
            double areaCount = 0;
            foreach (var item in hts)
            {
                if (item.CBHTBM != null && codeColl.Contains(item.CBHTBM))
                    areaCount += ToolMath.ConvertRound(item.HTZMJ * 0.0015, 2);
            }
            return ToolMath.ConvertRound(areaCount, 2);
        }

        /// <summary>
        /// 统计实测亩面积
        /// </summary>
        static public double AreaSCSum(IEnumerable<SqliteDK> dks)
        {
            double areaCount = 0;
            foreach (var item in dks)
            {
                areaCount += ToolMath.ConvertRound(item.SCMJ * 0.0015, 2);
            }
            return ToolMath.ConvertRound(areaCount, 2);
        }

        /// <summary>
        /// 统计确权亩面积
        /// </summary>
        static public double AreaQQSum(IEnumerable<SqliteDK> dks, Dictionary<string, double> areaDic)
        {
            double areaCount = 0;
            foreach (var item in dks)
            {
                item.DKBM = item.DKBM == null ? "" : item.DKBM;
                if (areaDic.ContainsKey(item.DKBM))
                    areaCount += ToolMath.ConvertRound(areaDic[item.DKBM] * 0.0015, 2);
            }
            return ToolMath.ConvertRound(areaCount, 2);
        }

        /// <summary>
        /// 导出统计表格
        /// </summary>
        public bool ExportTable(bool exportProperty = true)
        {
            try
            {
                if (ExportFile == null)
                    ExportFile = new ExportFileEntity();

                if (ExportFile.SumTableLand.IsExport)
                    ExportLandSummeryTable();

                if (ExportFile.SumTableUsefor.IsExport)
                    ExportLandUseforTable();

                if (ExportFile.SumTableOwnType.IsExport && exportProperty)
                    ExportLandPropertyTable();

                if (ExportFile.SumTableLandType.IsExport)
                    ExportLandCalssTable();

                if (ExportFile.SumTableIsBase.IsExport)
                    ExportLandIsBaseTable();

                if (ExportFile.SumTableWarrant.IsExport)
                    ExportRegisterbookTable();

                if (ExportFile.SumTableContracter.IsExport)
                    ExportContractorTable();
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("导出汇总表格问题：", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 导出地块汇总表
        /// </summary>
        public void ExportLandSummeryTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按地块汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            SummeryTableHeader(sheet);
            ExportExcel(sheet, new List<string>() {DataSummary.UnitCodeName,DataSummary.UnitNameName,DataSummary.SenderCountName,
                DataSummary.ContractLandCountName,DataSummary.ContractLandAreaCountName,DataSummary.UnContractLandCountName,DataSummary.UnContractLandAreaCountName,DataSummary.RegisterBookNumberName }, 4, false);
            provider.Save();
        }

        /// <summary>
        /// 地块汇总表表头
        /// </summary>
        private void SummeryTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            IRow row3 = sheet.Rows[3];
            sheet.MergedRegions.Create(0, 0, 0, 7);
            sheet.MergedRegions.Create(1, 1, 0, 7);
            sheet.MergedRegions.Create(2, 3, 0, 0);
            sheet.MergedRegions.Create(2, 3, 1, 1);
            sheet.MergedRegions.Create(2, 3, 2, 2);
            sheet.MergedRegions.Create(2, 2, 3, 4);
            sheet.MergedRegions.Create(2, 2, 5, 6);
            sheet.MergedRegions.Create(2, 3, 7, 7);
            row0.Cells[0].Value = "按地块汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:个;块;亩;份";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "发包方数量";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[3].Value = "承包地块";
            row2.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[5].Value = "非承包地块";
            row2.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[7].Value = "颁发权证数量";
            row2.Cells[7].HorizontalAlignment = eHorizontalAlignment.Center;

            row3.Cells[3].Value = "总数";
            row3.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[4].Value = "总面积";
            row3.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[5].Value = "总数";
            row3.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[6].Value = "总面积";
            row3.Cells[6].HorizontalAlignment = eHorizontalAlignment.Center;
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[7].Width = 210;
        }

        /// <summary>
        /// 导出表格
        /// </summary>
        private void ExportExcel(ISheet sheet, List<string> colums, int startindex = 4, bool removeNoperson = true)
        {
            if (sheet == null)
            {
                return;
            }
            Type type = typeof(DataSummary);
            int index = startindex;
            foreach (var item in sumList)
            {
                if (item.FamilyCount == 0 && removeNoperson && item.Level < eZoneLevel.Town)
                    continue;
                IRow row = sheet.Rows[index];
                int colIndex = 0;
                foreach (string col in colums)
                {
                    ICell cell = row.Cells[colIndex];
                    PropertyInfo info = type.GetProperty(col);
                    object value = info.GetValue(item, null);
                    if (info.PropertyType == typeof(double))
                    {
                        double cellValue = ToolMath.Round(((double)value));// / 10000);
                        //var cl = row.Instance.CreateCell(colIndex);
                        //cl.SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        cell.Instance.SetCellValue(cellValue);
                    }
                    else if (info.PropertyType == typeof(int))
                    {
                        cell.Instance.SetCellValue((int)value);
                    }
                    else
                    {
                        cell.Instance.SetCellValue((string)value);
                    }
                    colIndex++;
                }
                index++;
            }
        }

        /// <summary>
        /// 导出土地用途汇总表
        /// </summary>
        public void ExportLandUseforTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按承包地土地用途汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            UseforTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.AgricureAreaCountName,DataSummary.PlantAreaCountName,DataSummary.ForestAreaCountName,
            DataSummary.AnimalAreaCountName,DataSummary.FishAreaCountName,DataSummary.UnAgricureAreaCountName,
            DataSummary.AgricureAndUnAreaCountName});
            provider.Save();
        }

        /// <summary>
        /// 土地用途汇总表表头
        /// </summary>
        private void UseforTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            IRow row3 = sheet.Rows[3];
            sheet.MergedRegions.Create(0, 0, 0, 8);
            sheet.MergedRegions.Create(1, 1, 0, 8);
            sheet.MergedRegions.Create(2, 3, 0, 0);
            sheet.MergedRegions.Create(2, 3, 1, 1);
            sheet.MergedRegions.Create(2, 2, 2, 6);
            sheet.MergedRegions.Create(2, 3, 7, 7);
            sheet.MergedRegions.Create(2, 3, 8, 8);
            row0.Cells[0].Value = "按承包地土地用途汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:亩";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "农业用途面积";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[7].Value = "非农用途面积";
            row2.Cells[7].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[8].Value = "合计";
            row2.Cells[8].HorizontalAlignment = eHorizontalAlignment.Center;

            row3.Cells[2].Value = "合计";
            row3.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[3].Value = "种植业";
            row3.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[4].Value = "林业";
            row3.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[5].Value = "畜牧业";
            row3.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[6].Value = "渔业";
            row3.Cells[6].HorizontalAlignment = eHorizontalAlignment.Center;
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[7].Width = 180;
        }

        /// <summary>
        /// 导出土地性质汇总表
        /// </summary>
        public void ExportLandPropertyTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按承包地所有权性质汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            PropertyTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.CollectivityAreaName,DataSummary.GroupAreaCountName,DataSummary.GroupCollectivityAreaName,
            DataSummary.TownCollectivityAreaName,DataSummary.OtherCollectivityAreaName,DataSummary.CountryAreaCountName,
            DataSummary.CountryAndCollAreaCountName});
            provider.Save();
        }

        /// <summary>
        /// 土地性质汇总表表头
        /// </summary>
        private void PropertyTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            IRow row3 = sheet.Rows[3];
            row2.Height = 25;
            row3.Height = 27;
            sheet.MergedRegions.Create(0, 0, 0, 8);
            sheet.MergedRegions.Create(1, 1, 0, 8);
            sheet.MergedRegions.Create(2, 3, 0, 0);
            sheet.MergedRegions.Create(2, 3, 1, 1);
            sheet.MergedRegions.Create(2, 2, 2, 6);
            sheet.MergedRegions.Create(2, 3, 7, 7);
            sheet.MergedRegions.Create(2, 3, 8, 8);
            row0.Cells[0].Value = "按承包地所有权性质汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:亩";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].WrapText = true;
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "集体土地所有权面积";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[7].Value = "国有土地所有权面积";
            row2.Cells[7].WrapText = true;
            row2.Cells[7].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[8].Value = "合计";
            row2.Cells[8].HorizontalAlignment = eHorizontalAlignment.Center;

            row3.Cells[2].Value = "合计";
            row3.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[3].Value = "村民小组";
            row3.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[4].Value = "村级集体经济组织";
            row3.Cells[4].WrapText = true;
            row3.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[5].Value = "乡级集体经济组织";
            row3.Cells[5].WrapText = true;
            row3.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[6].Value = "其他农民集体经济组织";
            row3.Cells[6].WrapText = true;
            row3.Cells[6].HorizontalAlignment = eHorizontalAlignment.Center;
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[6].Width = 160;
        }

        /// <summary>
        /// 导出非承包地类别汇总表
        /// </summary>
        public void ExportLandCalssTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按非承包地地块类别汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            CalssTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.SelfLandAreaCountName,DataSummary.MoveLandAreaCountName,DataSummary.WastLandAreaCountName,
            DataSummary.OtherCollectivityLandAreaName,DataSummary.UnContractLandTypeAreaName}, 3, false);
            provider.Save();
        }

        /// <summary>
        /// 非承包地类别汇总表表头
        /// </summary>
        private void CalssTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            sheet.MergedRegions.Create(0, 0, 0, 6);
            sheet.MergedRegions.Create(1, 1, 0, 6);
            sheet.Columns[5].Width = 165;
            row0.Cells[0].Value = "按非承包地地块类别汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:亩";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].WrapText = true;
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "自留地";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[3].Value = "机动地";
            row2.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[4].Value = "开荒地";
            row2.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[5].Value = "其他集体土地";
            row2.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[6].Value = "合计";
            row2.Cells[6].HorizontalAlignment = eHorizontalAlignment.Center;

            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
        }

        /// <summary>
        /// 承包地是否基本农田汇总表
        /// </summary>
        public void ExportLandIsBaseTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按承包地是否基本农田汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            IsBaseTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.ContractIsBaseAreaName,DataSummary.ContractNotBaseAreaName,DataSummary.ContractBaseAreaCountName}, 3);
            provider.Save();
        }

        /// <summary>
        /// 否基本农田汇总表表头
        /// </summary>
        private void IsBaseTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            row2.Height = 25;
            sheet.Columns[5].Width = 165;
            sheet.MergedRegions.Create(0, 0, 0, 4);
            sheet.MergedRegions.Create(1, 1, 0, 4);
            row0.Cells[0].Value = "按承包地是否基本农田汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:亩";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].WrapText = true;
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "基本农田面积";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[3].Value = "非基本农田面积";
            row2.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[4].Value = "合计";
            row2.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;

            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[2].Width = 220;
            sheet.Columns[3].Width = 220;
        }

        /// <summary>
        /// 权证信息汇总表
        /// </summary>
        public void ExportRegisterbookTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按权证信息汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            RegisterbookTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.RegisterBookNumberName,DataSummary.FamilyContractBookNumberName,DataSummary.OtherContractBookNumberName,
            DataSummary.GisterbookAreaName});
            provider.Save();
        }

        /// <summary>
        /// 权证信息汇总表表头
        /// </summary>
        private void RegisterbookTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            IRow row3 = sheet.Rows[3];
            row2.Height = 23;
            row3.Height = 23;
            sheet.MergedRegions.Create(0, 0, 0, 5);
            sheet.MergedRegions.Create(1, 1, 0, 5);
            sheet.MergedRegions.Create(2, 3, 0, 0);
            sheet.MergedRegions.Create(2, 3, 1, 1);
            sheet.MergedRegions.Create(2, 2, 2, 4);
            sheet.MergedRegions.Create(2, 3, 5, 5);
            row0.Cells[0].Value = "按权证信息汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:份;亩";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].WrapText = true;
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "颁发权证数量";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[5].Value = "颁发权证面积";
            row2.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;

            row3.Cells[2].Value = "合计";
            row3.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[3].Value = "家庭承包";
            row3.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[4].Value = "其他方式承包";
            row3.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[2].Width = 120;
            sheet.Columns[3].Width = 130;
            sheet.Columns[4].Width = 150;
            sheet.Columns[5].Width = 170;
        }

        /// <summary>
        /// 承包方汇总表
        /// </summary>
        public void ExportContractorTable()
        {
            string fileName = Path.Combine(filePath, codeName + "按承包方汇总表.xls");
            IProviderExcelFile provider = CreateSheet(fileName);
            if (provider == null)
            {
                return;
            }
            ISheet sheet = provider.Sheets.Create("汇总表");
            ContractorTableHeader(sheet);
            ExportExcel(sheet, new List<string>() { DataSummary.UnitCodeName, DataSummary.UnitNameName,
            DataSummary.FamilyCountName,DataSummary.ContractFamilyCountName,DataSummary.ContractFamilyPersonCountName,
            DataSummary.OtherFamilyCountName,DataSummary.UnitFamilyCountName,DataSummary.PersonalFamilyCountName});
            provider.Save();
        }

        /// <summary>
        /// 承包方汇总表表头
        /// </summary>
        private void ContractorTableHeader(ISheet sheet)
        {
            IRow row0 = sheet.Rows[0];
            IRow row1 = sheet.Rows[1];
            IRow row2 = sheet.Rows[2];
            IRow row3 = sheet.Rows[3];
            row2.Height = 27;
            row3.Height = 27;
            sheet.MergedRegions.Create(0, 0, 0, 7);
            sheet.MergedRegions.Create(1, 1, 0, 7);
            sheet.MergedRegions.Create(2, 3, 0, 0);
            sheet.MergedRegions.Create(2, 3, 1, 1);
            sheet.MergedRegions.Create(2, 3, 2, 2);
            sheet.MergedRegions.Create(2, 2, 3, 4);
            sheet.MergedRegions.Create(2, 2, 5, 7);
            row0.Cells[0].Value = "按承包方汇总表";
            row0.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row0.Cells[0].FontSize = 14;
            row1.Cells[0].Value = "单位:个";
            row1.Cells[0].HorizontalAlignment = eHorizontalAlignment.Right;
            row2.Cells[0].Value = "权属单位代码";
            row2.Cells[0].WrapText = true;
            row2.Cells[0].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[1].Value = "权属单位名称";
            row2.Cells[1].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[2].Value = "承包方总数";
            row2.Cells[2].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[3].Value = "家庭承包";
            row2.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row2.Cells[5].Value = "其他方式承包";
            row2.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;

            row3.Cells[3].Value = "承包农户数量";
            row3.Cells[3].WrapText = true;
            row3.Cells[3].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[4].Value = "家庭成员数量";
            row3.Cells[4].WrapText = true;
            row3.Cells[4].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[5].Value = "合计";
            row3.Cells[5].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[6].Value = "单位承包数量";
            row3.Cells[6].WrapText = true;
            row3.Cells[6].HorizontalAlignment = eHorizontalAlignment.Center;
            row3.Cells[7].Value = "个人承包数量";
            row3.Cells[7].WrapText = true;
            row3.Cells[7].HorizontalAlignment = eHorizontalAlignment.Center;
            sheet.Columns[0].Width = 358;
            sheet.Columns[1].Width = 475;
            sheet.Columns[2].Width = 120;
            sheet.Columns[3].Width = 130;
            sheet.Columns[4].Width = 150;
            sheet.Columns[5].Width = 70;
            sheet.Columns[6].Width = 170;
            sheet.Columns[7].Width = 170;
        }

        /// <summary>
        /// 初始化统计信息
        /// </summary>
        private void InitialSummary()
        {
            List<DataSummary> grouplist = sumList.FindAll(t => t.Level == eZoneLevel.Group);
            List<DataSummary> villagelist = sumList.FindAll(t => t.Level == eZoneLevel.Village);
            List<DataSummary> townlist = sumList.FindAll(t => t.Level == eZoneLevel.Town);
            List<DataSummary> countylist = sumList.FindAll(t => t.Level == eZoneLevel.County);
            List<DataSummary> insertSummary = new List<DataSummary>();
            foreach (var sum in villagelist)
            {
                if (hasValue(sum))
                {
                    var index = sumList.IndexOf(sum);
                    var newsum = new DataSummary() { UnitCode = sum.UnitCode, UnitName = sum.UnitName, ZoneCode = sum.ZoneCode, Level = sum.Level };
                    newsum.Add(sum);
                    List<DataSummary> list = grouplist.FindAll(t => t.ZoneCode.StartsWith(sum.ZoneCode));
                    list.ForEach(t => newsum.Add(t));
                    sumList.Insert(index, newsum);
                    insertSummary.Add(newsum);
                }
                else
                {
                    List<DataSummary> list = grouplist.FindAll(t => t.ZoneCode.StartsWith(sum.ZoneCode));
                    list.ForEach(t => sum.Add(t));
                }
            }
            foreach (var sum in townlist)
            {
                List<DataSummary> list = villagelist.FindAll(t => t.ZoneCode.StartsWith(sum.ZoneCode));
                list.ForEach(t =>
                {
                    var newsum = insertSummary.Find(s => s.ZoneCode == t.ZoneCode && s.UnitCode == t.UnitCode);
                    if (newsum != null)
                    {
                        sum.Add(newsum);
                    }
                    else
                    {
                        sum.Add(t);
                    }
                });
            }
            foreach (var sum in countylist)
            {
                List<DataSummary> list = townlist.FindAll(t => t.ZoneCode.StartsWith(sum.ZoneCode));
                list.ForEach(t => sum.Add(t));
            }
        }

        private bool hasValue(DataSummary ds)
        {
            bool result = false;
            if (ds.FamilyCount == 0 && ds.UnAgricureAreaCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.UnContractLandAreaCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.UnContractLandCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.UnContractLandTypeArea > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.MoveLandAreaCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.SelfLandAreaCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.WastLandAreaCount > 0)
                result = true;
            if (ds.FamilyCount == 0 && ds.OtherCollectivityArea > 0)
                result = true;
            return result;
        }

        /// <summary>
        /// 创建表格
        /// </summary>
        /// <returns></returns>
        private IProviderExcelFile CreateSheet(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                }
            }
            var excelfielcs = new ExcelFileConnectionStringBuilder();
            excelfielcs.FileName = fileName;
            excelfielcs.FileMode = System.IO.FileMode.Create;
            excelfielcs.FileAccess = System.IO.FileAccess.ReadWrite;
            var provider = new ProviderExcelFile(excelfielcs.ConnectionString);
            return provider;
        }

        #endregion
    }
}
