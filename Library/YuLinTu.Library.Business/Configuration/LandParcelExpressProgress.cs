using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    public class LandParcelExpressProgress : WordBase
    {
        #region Propertys

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        public ContractBusinessParcelWordSettingDefine SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 地块信息
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageSize { get; set; }

        public string OtherInfo { get; set; }

        /// <summary>
        /// 批量导出
        /// </summary>
        public bool BatchExport { get; set; }

        /// <summary>
        /// 宗地图文件保存路径
        /// </summary>
        public string SavePathOfImage { get; set; }

        #endregion Propertys

        #region Ctor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public LandParcelExpressProgress()
        {
        }

        #endregion Ctor

        /// <summary>
        /// 设置书签值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                if (LandCollection != null && LandCollection.Count > 0)
                {
                    SetContractLandValue(LandCollection);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Method - Land

        public void InitalizeAgriLandExpress(bool isPrint, string filePath)
        {
            if (!InitalizeAgriLandTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在共有人模版信息或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                if (!isPrint)
                {
                    int familyNumber = 0;
                    if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
                    {
                        Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
                    }
                    else
                    {
                        ContractLand land = (LandCollection != null && LandCollection.Count > 0) ? LandCollection[0] : null;
                        string landNumber = land != null ? ContractLand.GetLandNumber(land.CadastralNumber) : "";
                        landNumber = (!string.IsNullOrEmpty(landNumber) && landNumber.Length >= 18) ? landNumber.Substring(14, 3) : "";
                        if (!string.IsNullOrEmpty(landNumber))
                        {
                            Int32.TryParse(landNumber, out familyNumber);
                        }
                    }
                    string familyName = (familyNumber == 0 ? Contractor.Name : (familyNumber + "-" + Contractor.Name));
                    string file = Path.Combine(filePath, "农村土地承包经营权标准地块示意图扩展");

                    if (!Directory.Exists(file))
                    {
                        Directory.CreateDirectory(file);
                    }

                    file = Path.Combine(file, familyName + "-农村土地承包经营权标准地块示意图扩展");
                    this.SaveAs(this, file, true);
                }
                else
                {
                    this.PrintPreview(this, SystemSet.DefaultPath + @"\" + Contractor.Name + " -农村土地承包经营权标准地块示意图扩展");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool InitalizeAgriLandTemplateFilePath()
        {
            string filePath = TemplateHelper.WordTemplate("农村土地承包经营权标准地块示意图扩展");
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!OpenTemplate(filePath))//打开文件
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="dt"></param>
        private void SetContractLandValue(List<ContractLand> landCollection)
        {
            var landCount = LandCollection.Count;
            var pageCount = SettingDefine.ExtendRowCount * SettingDefine.ExtendColCount;
            var pageSize = landCount % pageCount == 0
                ? landCount / pageCount
                : landCount / pageCount + 1;

            for (int i = 1; i < pageSize; i++)
            {
                // 复制页
                AddSection();
            }

            for (int i = 0; i < pageSize; i++)
            {
                // 写入制图者等信息
                SetTableCellValue(i, 0, 1, SettingDefine.ExtendRowCount, OtherInfo);
                SetTableCellValue(i, 0, 0, SettingDefine.ExtendRowCount, (i + PageIndex).ToString() + "-" + TotalPageSize.ToString());

                // 写入地块示意图图片
                InitalizeLandImageInformation(landCollection.Skip(pageCount * i).Take(pageCount).ToList(), i, 0,
                    SettingDefine.ExtendRowCount, SettingDefine.ExtendColCount, 0, 0, true);
            }
        }

        /// <summary>
        /// 将示意图写入对应表格
        /// </summary>
        /// <param name="lands">地块</param>
        /// <param name="tableIndex">表格索引</param>
        /// <param name="row">表格行数</param>
        /// <param name="col">表格列数</param>
        /// <param name="isHorizontal">是否横版</param>
        private void InitalizeLandImageInformation(List<ContractLand> lands, int sectionIndex, int tableIndex, int row, int col, int startRow, int startCol, bool isHorizontal = false)
        {
            if (lands == null || lands.Count == 0)
            {
                return;
            }
            int landIndex = 0;
            if (isHorizontal)
            {
                for (int colInex = startRow; colInex < row + startRow; colInex++)
                {
                    for (int rowIndex = col - 1 + startCol; rowIndex >= startCol; rowIndex--)
                    {
                        if (landIndex >= lands.Count)
                        {
                            return;
                        }
                        ContractLand land = lands[landIndex];
                        InsertImageShape(land, sectionIndex, tableIndex, rowIndex, colInex);
                        landIndex++;
                    }
                }
            }
            else
            {
                for (int rowIndex = startRow; rowIndex < row + startRow; rowIndex++)
                {
                    for (int colInex = startCol; colInex < col + startCol; colInex++)
                    {
                        if (landIndex >= lands.Count)
                        {
                            return;
                        }
                        ContractLand land = lands[landIndex];
                        InsertImageShape(land, sectionIndex, tableIndex, rowIndex, colInex);
                        landIndex++;
                    }
                }
            }
        }

        private void InsertImageShape(ContractLand land, int sectionIndex, int tableIndex, int rowIndex, int columnIndex)
        {
            try
            {
                var uselandnumber = Regex.Replace(land.LandNumber, @"[^\d]", "_");
                string imagePath = SavePathOfImage + @"\" + uselandnumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    if (SettingDefine.IsFixedExtendLandGeoWord)
                    {
                        //if (SettingDefine.HorizontalVersion)
                        SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, 180, 115, -90, false);
                        //else
                        //SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, 80, 110, false);
                    }
                    else
                    {
                        //if (SettingDefine.HorizontalVersion)
                        SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, SettingDefine.ExtendLandGeoWordHeight, SettingDefine.ExtendLandGeoWordWidth, -90, false);
                        //else
                        //SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, SettingDefine.ExtendLandGeoWordWidth, SettingDefine.ExtendLandGeoWordHeight, false);
                    }
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion Method - Land
    }
}